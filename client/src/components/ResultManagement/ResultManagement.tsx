import React, { useState, useEffect } from 'react';
import commonStyles from '../Admin/AdminCommon.module.css';
import { 
  SessionDto, TermDto, ClassLevelDto, SubjectDto, 
  BulkScoreEntryDto, AssessmentDetailsDto 
} from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { resultService } from '../../services/resultService';
import { ValidationMessage } from '../Shared/ValidationComponents';
import ReportCardView from './ReportCardView';
import PrintableReportCard from './PrintableReportCard';
import ResultUpload from './ResultUpload';
import BroadsheetView from './BroadsheetView';

const ResultManagement: React.FC = () => {
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [terms, setTerms] = useState<TermDto[]>([]);
  const [classes, setClasses] = useState<ClassLevelDto[]>([]);
  const [subjects, setSubjects] = useState<SubjectDto[]>([]);

  const [selectedSession, setSelectedSession] = useState('');
  const [selectedTerm, setSelectedTerm] = useState('');
  const [selectedClass, setSelectedClass] = useState('');
  const [selectedSubject, setSelectedSubject] = useState('');

  const [students, setStudents] = useState<any[]>([]);
  const [scores, setScores] = useState<Record<string, { testScore: string, examScore: string }>>({});
  
  const [loading, setLoading] = useState(false);
  const [loadingInitial, setLoadingInitial] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  
  const [selectedAssessmentId, setSelectedAssessmentId] = useState<string | null>(null);
  const [showUpload, setShowUpload] = useState(false);
  const [printAssessmentId, setPrintAssessmentId] = useState<string | null>(null);
  const [printAssessmentReport, setPrintAssessmentReport] = useState<AssessmentDetailsDto | null>(null);
  const [showBroadsheet, setShowBroadsheet] = useState(false);
  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        const [sessResp, classResp, subResp] = await Promise.all([
          academicStructureService.getSessions(),
          academicStructureService.getClassLevels(),
          academicStructureService.getSubjects()
        ]);
        
        if (sessResp.isSuccess) setSessions(sessResp.data || []);
        if (classResp.isSuccess) setClasses(classResp.data || []);
        if (subResp.isSuccess) setSubjects(subResp.data || []);
      } catch (err: any) {
        const serverMsg = err.response?.data?.message || err.response?.data?.errors?.[0] || err.message;
        setError('Failed to load academic structure: ' + serverMsg);
      } finally {
        setLoadingInitial(false);
      }
    };
    fetchInitialData();
  }, []);

  useEffect(() => {
    const fetchTerms = async () => {
      if (!selectedSession) {
        setTerms([]);
        setSelectedTerm('');
        return;
      }
      try {
        const termResp = await academicStructureService.getTermsBySession(selectedSession);
        if (termResp.isSuccess) {
          setTerms(termResp.data || []);
          if (termResp.data && termResp.data.length > 0) {
            setSelectedTerm(termResp.data[0].id);
          } else {
            setSelectedTerm('');
          }
        }
      } catch (err: any) {
        console.error(err);
      }
    };
    fetchTerms();
  }, [selectedSession]);

  const handleFetchStudents = async () => {
    if (!selectedSession || !selectedTerm || !selectedClass || !selectedSubject) {
      setError('Please select Session, Term, Class, and Subject to load students.');
      return;
    }

    setLoading(true);
    setError('');
    setSuccess('');
    
    try {
      const assessments = await resultService.getClassResults(selectedSession, selectedTerm, selectedClass);
      setStudents(assessments as any);
        
      // Initialize scores state
      const initialScores: Record<string, { testScore: string, examScore: string }> = {};
      assessments.forEach(stu => {
        initialScores[stu.examTakerId] = { testScore: '', examScore: '' };
      });
      setScores(initialScores);
      
      if (assessments.length === 0) {
        setError('No students found for this class in the selected session/term.');
      }
    } catch (err: any) {
      setError('Failed to fetch students: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleScoreChange = (examTakerId: string, field: 'testScore' | 'examScore', value: string) => {
    // Validate max scores: Test 40, Exam 60
    let numVal = parseInt(value);
    if (!isNaN(numVal)) {
      if (field === 'testScore' && numVal > 40) numVal = 40;
      if (field === 'examScore' && numVal > 60) numVal = 60;
      if (numVal < 0) numVal = 0;
      value = numVal.toString();
    }

    setScores(prev => ({
      ...prev,
      [examTakerId]: {
        ...prev[examTakerId],
        [field]: value
      }
    }));
  };

  const handleSaveScores = async () => {
    if (!selectedSession || !selectedTerm || !selectedClass || !selectedSubject) {
      setError('Missing selections.');
      return;
    }

    setLoading(true);
    setError('');
    setSuccess('');

    const payload: BulkScoreEntryDto = {
      sessionId: selectedSession,
      termId: selectedTerm,
      classLevelId: selectedClass,
      subjectId: selectedSubject,
      scores: students.map(stu => ({
        examTakerId: stu.examTakerId,
        subjectId: selectedSubject,
        testScore: scores[stu.examTakerId]?.testScore !== '' ? parseInt(scores[stu.examTakerId].testScore) : undefined,
        examScore: scores[stu.examTakerId]?.examScore !== '' ? parseInt(scores[stu.examTakerId].examScore) : undefined,
      }))
    };

    try {
      const resp = await resultService.saveBulkScores(payload);
      if (resp.isSuccess) {
        setSuccess('Scores saved successfully.');
      } else {
        setError(resp.message || 'Failed to save scores.');
      }
    } catch (err: any) {
      setError('An error occurred while saving scores.');
    } finally {
      setLoading(false);
    }
  };

  const handleCalculateClass = async () => {
    if (!selectedSession || !selectedTerm || !selectedClass) {
      setError('Please select Session, Term, and Class to calculate results.');
      return;
    }
    
    if (!window.confirm("This will calculate total scores, averages, and class rankings. Continue?")) return;

    setLoading(true);
    setError('');
    setSuccess('');

    try {
      const resp = await resultService.calculateClassResults(selectedSession, selectedTerm, selectedClass);
      if (resp.isSuccess) {
        setSuccess('Class results calculated and ranked successfully.');
        handleFetchStudents(); // Refresh data
      } else {
        setError(resp.message || 'Failed to calculate class results.');
      }
    } catch (err: any) {
      setError('Error occurred: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSyncOnlineScores = async () => {
    if (!selectedSession || !selectedTerm || !selectedClass) {
      setError('Please select Session, Term, and Class to sync online scores.');
      return;
    }
    
    if (!window.confirm("This will fetch and sync scores from all completed online exams for this class. Existing exam scores for these subjects will be overwritten. Continue?")) return;

    setLoading(true);
    setError('');
    setSuccess('');

    try {
      const resp = await resultService.syncOnlineScores(selectedSession, selectedTerm, selectedClass);
      if (resp.isSuccess) {
        setSuccess(resp.message || 'Online scores synchronized successfully.');
        handleFetchStudents(); // Refresh the grid
      } else {
        setError(resp.message || 'Failed to sync online scores.');
      }
    } catch (err: any) {
      setError('Error occurred during sync: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handlePrint = async (id: string) => {
    setLoading(true);
    setError('');
    try {
      const resp = await resultService.getReportCard(id);
      if (resp) {
        setPrintAssessmentReport(resp as any);
        setPrintAssessmentId(id);
      } else {
        setError('Failed to load report for printing.');
      }
    } catch (err: any) {
      setError('Error loading report: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleToggleLock = async (id: string, currentLockState: boolean) => {
    setLoading(true);
    setError('');
    try {
      const resp = await resultService.toggleAssessmentLock(id, !currentLockState);
      if (resp.isSuccess) {
        setSuccess(`Result ${!currentLockState ? 'locked' : 'unlocked'} successfully.`);
        // Optimistically update the local state to avoid full refetch
        setStudents((prev: any[]) => prev.map((s: any) => s.id === id ? { ...s, isLockedForParents: !currentLockState } : s));
      } else {
        setError(resp.message || 'Failed to toggle lock.');
      }
    } catch (err: any) {
      setError('Error toggling lock: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleBatchStatusUpdate = async (currentStatus: number, newStatus: number) => {
    if (!selectedSession || !selectedTerm || !selectedClass) {
      setError('Please select Session, Term, and Class.');
      return;
    }
    
    setLoading(true);
    try {
      // @ts-ignore Since enum maps to numbers, passing the number is fine
      const resp = await resultService.batchUpdateClassStatus(selectedSession, selectedTerm, selectedClass, currentStatus, newStatus);
      if (resp.isSuccess) {
        setSuccess(`Moderation status updated successfully!`);
        handleFetchStudents();
      } else {
        setError(resp.message || 'Failed to update status.');
      }
    } catch (err: any) {
      setError('Error updating status: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  if (loadingInitial) return <div>Loading...</div>;

  return (
    <div className={commonStyles.container}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <h2 className={commonStyles.title}>Result Office</h2>
          <p className={commonStyles.description}>
            Bulk enter scores, review, and moderate student assessments.
          </p>
        </div>
        <div style={{ display: 'flex', gap: '12px' }}>
          <button 
            onClick={() => setShowBroadsheet(true)}
            style={{ padding: '8px 16px', backgroundColor: '#8b5cf6', color: 'white', border: 'none', borderRadius: '6px', fontWeight: 600, cursor: 'pointer' }}
            disabled={!selectedClass}
          >
            Class Broadsheet
          </button>
          <button 
            onClick={() => setShowUpload(true)}
            style={{ padding: '8px 16px', backgroundColor: '#3b82f6', color: 'white', border: 'none', borderRadius: '6px', fontWeight: 600, cursor: 'pointer' }}
            disabled={!selectedClass}
          >
            Bulk Upload CSV
          </button>
          <button 
            onClick={handleSyncOnlineScores}
            style={{ padding: '8px 16px', backgroundColor: '#10b981', color: 'white', border: 'none', borderRadius: '6px', fontWeight: 600, cursor: 'pointer' }}
            disabled={loading || !selectedClass}
            title="Fetch and sync scores from completed online exams"
          >
            {loading ? 'Syncing...' : 'Sync Online Scores'}
          </button>
        </div>
      </div>

      {showUpload && (
        <div style={modalOverlayStyle}>
          <ResultUpload 
            sessionId={selectedSession}
            termId={selectedTerm}
            classLevelId={selectedClass}
            onSuccess={() => {
                setSuccess('Results uploaded successfully!');
                handleFetchStudents();
            }}
            onClose={() => setShowUpload(false)}
          />
        </div>
      )}

      <div className={commonStyles.card} style={{ marginBottom: '24px', padding: '20px' }}>
        <h3 style={{ marginTop: 0, fontSize: '16px' }}>Filter Context</h3>
        <div style={{ display: 'flex', gap: '16px', flexWrap: 'wrap' }}>
          <div style={{ flex: 1, minWidth: '200px' }}>
            <label style={{ display: 'block', fontSize: '14px', fontWeight: 600, marginBottom: '8px' }}>Session</label>
            <select 
              style={selectStyle} 
              value={selectedSession} 
              onChange={e => setSelectedSession(e.target.value)}
            >
              <option value="">-- Select Session --</option>
              {sessions.map(s => <option key={s.id} value={s.id}>{s.name} {s.isActive ? '(Active)' : ''}</option>)}
            </select>
          </div>
          <div style={{ flex: 1, minWidth: '200px' }}>
            <label style={{ display: 'block', fontSize: '14px', fontWeight: 600, marginBottom: '8px' }}>Term</label>
            <select 
              style={selectStyle} 
              value={selectedTerm} 
              onChange={e => setSelectedTerm(e.target.value)}
              disabled={!selectedSession}
            >
              <option value="">-- Select Term --</option>
              {terms.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
            </select>
          </div>
          <div style={{ flex: 1, minWidth: '200px' }}>
            <label style={{ display: 'block', fontSize: '14px', fontWeight: 600, marginBottom: '8px' }}>Class Level</label>
            <select 
              style={selectStyle} 
              value={selectedClass} 
              onChange={e => setSelectedClass(e.target.value)}
            >
              <option value="">-- Select Class --</option>
              {classes.map(c => <option key={c.id} value={c.id}>{c.name} {c.sectionOrArm}</option>)}
            </select>
          </div>
          <div style={{ flex: 1, minWidth: '200px' }}>
            <label style={{ display: 'block', fontSize: '14px', fontWeight: 600, marginBottom: '8px' }}>Subject</label>
            <select 
              style={selectStyle} 
              value={selectedSubject} 
              onChange={e => setSelectedSubject(e.target.value)}
            >
              <option value="">-- Select Subject --</option>
              {subjects.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
            </select>
          </div>
        </div>
        
        <div style={{ marginTop: '20px', display: 'flex', gap: '12px' }}>
          <button 
            onClick={handleFetchStudents}
            style={{ padding: '10px 20px', backgroundColor: '#3b82f6', color: 'white', border: 'none', borderRadius: '6px', fontWeight: 600, cursor: 'pointer' }}
            disabled={loading}
          >
            {loading ? 'Loading...' : 'Load Students for Entry'}
          </button>
          
          <button 
            onClick={handleCalculateClass}
            style={{ padding: '10px 20px', backgroundColor: '#8b5cf6', color: 'white', border: 'none', borderRadius: '6px', fontWeight: 600, cursor: 'pointer' }}
            disabled={loading || !selectedClass}
            title="Calculate overall grades and rankings for the class after all subject scores are entered"
          >
            Calculate Class Rankings
          </button>
        </div>
      </div>

      <div className={commonStyles.card} style={{ marginBottom: '24px', padding: '20px' }}>
        <h3 style={{ marginTop: 0, fontSize: '16px' }}>Result Office Moderation & Workflow</h3>
        <p style={{ fontSize: '14px', color: '#4b5563', marginBottom: '16px' }}>
          Batch update the status of academic results for the selected class to push them through the school's moderation pipeline.
          (Draft → Vetted → Approved → Finalized for Report Cards)
        </p>
        <div style={{ display: 'flex', gap: '12px', flexWrap: 'wrap' }}>
          <button 
            onClick={() => handleBatchStatusUpdate(0, 1)}
            style={{ padding: '8px 16px', backgroundColor: '#3b82f6', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer' }}
            disabled={loading || !selectedClass}
          >Submit Drafts for Moderation</button>
          <button 
            onClick={() => handleBatchStatusUpdate(1, 2)}
            style={{ padding: '8px 16px', backgroundColor: '#f59e0b', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer' }}
            disabled={loading || !selectedClass}
          >Approve Moderated Results</button>
          <button 
            onClick={() => handleBatchStatusUpdate(2, 3)}
            style={{ padding: '8px 16px', backgroundColor: '#10b981', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer' }}
            disabled={loading || !selectedClass}
          >Publish Approved Results (Visible to Parents)</button>
        </div>
      </div>

      {error && <ValidationMessage type="error" message={error} />}
      {success && <ValidationMessage type="success" message={success} />}

      {students.length > 0 && (
        <div className={commonStyles.card} style={{ padding: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
            <h3 style={{ margin: 0, fontSize: '16px' }}>
              Bulk Score Entry 
              <span style={{ fontSize: '14px', color: '#6b7280', fontWeight: 'normal', marginLeft: '12px' }}>
                {subjects.find(s => s.id === selectedSubject)?.name} - {classes.find(c => c.id === selectedClass)?.name}
              </span>
            </h3>
            <button 
              onClick={handleSaveScores}
              style={{ padding: '10px 20px', backgroundColor: '#10b981', color: 'white', border: 'none', borderRadius: '6px', fontWeight: 600, cursor: 'pointer' }}
              disabled={loading}
            >
              Save Scores
            </button>
          </div>

          <div style={{ overflowX: 'auto' }}>
            <table style={tableStyle}>
              <thead>
                <tr>
                  <th style={thStyle}>Admn No.</th>
                  <th style={thStyle}>Student Name</th>
                  <th style={thStyle}>Status</th>
                  <th style={thStyle}>Test Score (40)</th>
                  <th style={thStyle}>Exam Score (60)</th>
                  <th style={thStyle}>Total (100)</th>
                  <th style={thStyle}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {students.map(stu => {
                  const testStr = scores[stu.examTakerId]?.testScore || '0';
                  const examStr = scores[stu.examTakerId]?.examScore || '0';
                  const total = (parseInt(testStr) || 0) + (parseInt(examStr) || 0);
                  
                  const statusMap = ['Draft', 'Moderated', 'Approved', 'Published'];
                  const statusLabel = statusMap[stu.status] || 'Unknown';
                  
                  return (
                    <tr key={stu.id} style={trStyle}>
                      <td style={tdStyle}>{stu.admissionNumber || '-'}</td>
                      <td style={tdStyle}><strong>{stu.studentName}</strong></td>
                      <td style={tdStyle}>
                        <span style={{ fontSize: '12px', fontWeight: 600, padding: '4px 8px', borderRadius: '4px', backgroundColor: '#f3f4f6' }}>
                          {statusLabel}
                        </span>
                      </td>
                      <td style={tdStyle}>
                        <input 
                          type="number" 
                          min="0" max="40"
                          style={inputStyle}
                          value={scores[stu.examTakerId]?.testScore || ''}
                          onChange={(e) => handleScoreChange(stu.examTakerId, 'testScore', e.target.value)}
                        />
                      </td>
                      <td style={tdStyle}>
                        <input 
                          type="number" 
                          min="0" max="60"
                          style={inputStyle}
                          value={scores[stu.examTakerId]?.examScore || ''}
                          onChange={(e) => handleScoreChange(stu.examTakerId, 'examScore', e.target.value)}
                        />
                      </td>
                      <td style={tdStyle}>
                        <span style={{ fontWeight: 600, color: total < 40 ? '#dc2626' : '#16a34a' }}>
                          {total}
                        </span>
                      </td>
                      <td style={{ ...tdStyle, display: 'flex', gap: '8px' }}>
                        <button 
                          onClick={() => setSelectedAssessmentId(stu.id)}
                          style={{ padding: '6px 12px', backgroundColor: '#e5e7eb', color: '#374151', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '13px', fontWeight: 600 }}
                        >
                          Edit Details
                        </button>
                        <button 
                          onClick={() => handlePrint(stu.id)}
                          style={{ padding: '6px 12px', backgroundColor: '#dcfce7', color: '#166534', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '13px', fontWeight: 600 }}
                        >
                          Print Report
                        </button>
                        <button 
                          onClick={() => handleToggleLock(stu.id, stu.isLockedForParents)}
                          style={{ 
                            padding: '6px 12px', 
                            backgroundColor: stu.isLockedForParents ? '#fee2e2' : '#f3f4f6', 
                            color: stu.isLockedForParents ? '#991b1b' : '#374151', 
                            border: 'none', 
                            borderRadius: '4px', 
                            cursor: 'pointer', 
                            fontSize: '13px', 
                            fontWeight: 600 
                          }}
                          title={stu.isLockedForParents ? "Unlock Result for Parents" : "Lock Result from Parents"}
                        >
                          {stu.isLockedForParents ? 'Unlock 🔓' : 'Lock 🔒'}
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {selectedAssessmentId && (
        <div style={modalOverlayStyle}>
          <ReportCardView 
            assessmentId={selectedAssessmentId} 
            onClose={() => setSelectedAssessmentId(null)}
            onSaved={() => setSelectedAssessmentId(null)}
          />
        </div>
      )}

      {printAssessmentReport && printAssessmentId && (
        <PrintableReportCard 
          report={printAssessmentReport}
          termInfo={terms.find(t => t.id === selectedTerm)}
          sessionInfo={sessions.find(s => s.id === selectedSession)}
          onClose={() => setPrintAssessmentId(null)} 
        />
      )}

      {showBroadsheet && (
        <div style={modalOverlayStyle}>
          <BroadsheetView 
            sessionId={selectedSession}
            termId={selectedTerm}
            classLevelId={selectedClass}
            subjects={subjects}
            onClose={() => setShowBroadsheet(false)}
          />
        </div>
      )}
    </div>
  );
};

const modalOverlayStyle: React.CSSProperties = {
  position: 'fixed',
  top: 0, left: 0, right: 0, bottom: 0,
  backgroundColor: 'rgba(0,0,0,0.6)',
  display: 'flex',
  justifyContent: 'center',
  alignItems: 'center',
  zIndex: 1000,
  padding: '20px'
};

const selectStyle: React.CSSProperties = {
  width: '100%',
  padding: '10px 12px',
  borderRadius: '6px',
  border: '1px solid #d1d5db',
  backgroundColor: '#f9fafb',
  fontSize: '14px',
  outline: 'none',
  boxSizing: 'border-box'
};

const tableStyle: React.CSSProperties = {
  width: '100%',
  borderCollapse: 'collapse',
};

const thStyle: React.CSSProperties = {
  textAlign: 'left',
  padding: '12px 16px',
  backgroundColor: '#f3f4f6',
  borderBottom: '2px solid #e5e7eb',
  fontSize: '14px',
  fontWeight: 600,
  color: '#374151'
};

const trStyle: React.CSSProperties = {
  borderBottom: '1px solid #e5e7eb',
};

const tdStyle: React.CSSProperties = {
  padding: '12px 16px',
  fontSize: '14px',
  color: '#374151'
};

const inputStyle: React.CSSProperties = {
  width: '80px',
  padding: '8px',
  borderRadius: '6px',
  border: '1px solid #d1d5db',
  textAlign: 'center'
};

export default ResultManagement;
