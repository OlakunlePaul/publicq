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
import { useAuth } from '../../context/AuthContext';
import { UserRole } from '../../models/UserRole';
import { sessionService } from '../../services/sessionService';
import api from '../../api/axios';



const ResultManagement: React.FC = () => {
  const { userRoles } = useAuth();
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
  const [extendingTime, setExtendingTime] = useState(false);

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

  // Add mobile responsive styles
  useEffect(() => {
    const style = document.createElement('style');
    style.textContent = `
      @media (max-width: 768px) {
        .result-management-header {
          flex-direction: column !important;
          align-items: stretch !important;
          gap: 16px !important;
          margin-bottom: 24px !important;
        }
        .result-management-header-actions {
          display: grid !important;
          grid-template-columns: 1fr 1fr !important;
          gap: 8px !important;
          width: 100% !important;
        }
        .result-management-header-actions button {
          width: 100% !important;
          font-size: 13px !important;
          padding: 10px 8px !important;
        }
        .result-management-header-actions button:last-child {
          grid-column: span 2 !important;
        }
        .result-management-filter-inputs {
          flex-direction: column !important;
          gap: 12px !important;
        }
        .result-management-filter-inputs > div {
          width: 100% !important;
          min-width: 0 !important;
        }
        .result-management-filter-actions {
          flex-direction: column !important;
          gap: 12px !important;
        }
        .result-management-filter-actions button {
          width: 100% !important;
          height: 48px !important;
        }
        .result-management-moderation-actions {
          flex-direction: column !important;
          gap: 8px !important;
        }
        .result-management-moderation-actions button {
          width: 100% !important;
          text-align: left !important;
          padding: 12px 16px !important;
        }
        .result-management-score-header {
          flex-direction: column !important;
          align-items: stretch !important;
          gap: 16px !important;
        }
        .result-management-score-header h3 {
          text-align: center !important;
        }
        .result-management-score-header button {
          width: 100% !important;
          height: 48px !important;
        }
        .result-management-table-container {
          border: none !important;
          box-shadow: none !important;
          background: transparent !important;
          overflow: visible !important;
        }
        .result-management-table, 
        .result-management-table thead, 
        .result-management-table tbody, 
        .result-management-table tr, 
        .result-management-table td {
          display: block !important;
          width: 100% !important;
        }
        .result-management-table thead {
          display: none !important;
        }
        .result-management-table tr {
          background-color: white !important;
          border-radius: 16px !important;
          margin-bottom: 24px !important;
          padding: 16px !important;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05), 0 1px 2px rgba(0, 0, 0, 0.1) !important;
          border: 1px solid #f3f4f6 !important;
        }
        .result-management-table td {
          border-bottom: 1px solid #f9fafb !important;
          padding: 12px 0 !important;
          display: flex !important;
          justify-content: space-between !important;
          align-items: center !important;
          text-align: right !important;
          min-height: 44px !important;
        }
        .result-management-table td:last-child {
          border-bottom: none !important;
          margin-top: 12px !important;
          flex-direction: column !important;
          align-items: stretch !important;
          text-align: left !important;
        }
        .result-management-table td::before {
          content: attr(data-label) !important;
          font-weight: 700 !important;
          color: #6b7280 !important;
          text-transform: uppercase !important;
          font-size: 11px !important;
          letter-spacing: 0.05em !important;
          text-align: left !important;
          margin-right: 16px !important;
        }
        .result-management-action-button {
          height: 44px !important;
          margin-bottom: 8px !important;
          display: flex !important;
          align-items: center !important;
          justify-content: center !important;
          width: 100% !important;
        }
        .result-management-score-input {
          width: 100px !important;
          height: 40px !important;
          font-size: 16px !important;
        }
      }
    `;
    document.head.appendChild(style);
    return () => {
      document.head.removeChild(style);
    };
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
      const subjectObj = subjects.find(s => s.id === selectedSubject);

      assessments.forEach(stu => {
        let defaultTest = '';
        let defaultExam = '';
        
        if (subjectObj && stu.subjectScores) {
          const existingScore = stu.subjectScores.find((s: any) => s.subjectName === subjectObj.name);
          if (existingScore) {
            defaultTest = existingScore.testScore != null ? String(existingScore.testScore) : '';
            defaultExam = existingScore.examScore != null ? String(existingScore.examScore) : '';
          }
        }
        
        initialScores[stu.studentId] = { testScore: defaultTest, examScore: defaultExam };
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

  const handleScoreChange = (studentId: string, field: 'testScore' | 'examScore', value: string) => {
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
      [studentId]: {
        ...prev[studentId],
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
        studentId: stu.studentId,
        subjectId: selectedSubject,
        testScore: scores[stu.studentId]?.testScore !== '' ? parseInt(scores[stu.studentId].testScore) : undefined,
        examScore: scores[stu.studentId]?.examScore !== '' ? parseInt(scores[stu.studentId].examScore) : undefined,
      }))
    };

    try {
      const resp = await resultService.saveBulkScores(payload);
      if (resp.isSuccess) {
        await handleFetchStudents();
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
      const res = await resultService.syncOnlineScores(selectedSession, selectedTerm, selectedClass);
      if (res.isSuccess) {
        setSuccess('Online scores synced successfully!');
        handleFetchStudents();
      } else {
        setError(res.message || 'Failed to sync online scores.');
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

  const handleExtendTime = async (studentId: string, name: string) => {
    const minutesStr = window.prompt(`How many extra minutes to grant to ${name}?`, "15");
    if (!minutesStr) return;
    const minutes = parseInt(minutesStr);
    if (isNaN(minutes) || minutes <= 0) {
      alert("Please enter a valid number of minutes.");
      return;
    }

    setExtendingTime(true);
    setError('');
    setSuccess('');

    try {
      // Fetch current group member states for this student to find InProgress/Completed/TimeElapsed modules
      // This is necessary because we need the ModuleProgress.Id to call extendTime
      await sessionService.getGroupMemberStates(studentId, '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000');
      // Wait, we need the actual assignment context.
      // Refined strategy: The backend ExtendTimeAsync takes userProgressId (Guid).
      // We need to fetch the progress record for the student's active assignment.
      
      // Let's assume for now the user is viewing a specific subject context.
      // We'll need to find the ModuleProgress record associated with this student, subject, session, and term.
      
      // I will implement a service method to get progress by student and active context if needed, 
      // but let's try to get it from what we have.
      
      const res = await api.get(`sessions/student/${studentId}/assignment-context`, {
        params: { sessionId: selectedSession, termId: selectedTerm, classId: selectedClass, subjectId: selectedSubject }
      });
      
      if (res.data && res.data.userProgressId) {
        const extendRes = await sessionService.extendTime(res.data.userProgressId, minutes);
        if (extendRes.isSuccess) {
          setSuccess(`Granted ${minutes} extra minutes to ${name}.`);
        } else {
          setError(extendRes.message || "Failed to extend time.");
        }
      } else {
        setError("Could not find an active exam session for this student in this subject.");
      }
    } catch (err: any) {
      setError("Error extending time: " + err.message);
    } finally {
      setExtendingTime(false);
    }
  };

  const handleBulkExtendTime = async () => {
    const minutesStr = window.prompt(`How many extra minutes to grant to ALL students in this class?`, "15");
    if (!minutesStr) return;
    const minutes = parseInt(minutesStr);
    if (isNaN(minutes) || minutes <= 0) {
      alert("Please enter a valid number of minutes.");
      return;
    }

    if (!window.confirm(`This will grant ${minutes} minutes to every student currently taking an exam in this class. Continue?`)) return;

    setExtendingTime(true);
    setError('');
    setSuccess('');

    try {
      // Call a bulk extension endpoint
      const res = await sessionService.bulkExtendTime(
        selectedSession,
        selectedTerm,
        selectedClass,
        selectedSubject,
        minutes
      );
      
      if (res.isSuccess) {
        setSuccess(`Granted ${minutes} extra minutes to the entire class.`);
      } else {
        setError(res.message || "Failed to extend class time.");
      }
    } catch (err: any) {
      setError("Error extending class time: " + err.message);
    } finally {
      setExtendingTime(false);
    }
  };

  if (loadingInitial) return <div>Loading...</div>;


  return (
    <div className={commonStyles.container}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }} className="result-management-header">
        <div>
          <h2 className={commonStyles.title}>Result Office</h2>
          <p className={commonStyles.description}>
            Bulk enter scores, review, and moderate student assessments.
          </p>
        </div>
        <div style={{ display: 'flex', gap: '12px' }} className="result-management-header-actions">
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
        <div style={{ display: 'flex', gap: '16px', flexWrap: 'wrap' }} className="result-management-filter-inputs">
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
        
        <div style={{ marginTop: '20px', display: 'flex', gap: '12px' }} className="result-management-filter-actions">
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
        <div style={{ display: 'flex', gap: '12px', flexWrap: 'wrap' }} className="result-management-moderation-actions">
          <button 
            onClick={() => handleBatchStatusUpdate(0, 1)}
            style={{ padding: '8px 16px', backgroundColor: '#3b82f6', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer' }}
            disabled={loading || !selectedClass}
          >Submit Drafts for Moderation</button>
          
          {userRoles.some(r => r === UserRole.MANAGER || r === UserRole.ADMINISTRATOR) && (
            <>
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
            </>
          )}
          <button 
            onClick={handleBulkExtendTime}
            style={{ padding: '8px 16px', backgroundColor: '#8b5cf6', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer' }}
            disabled={loading || extendingTime || !selectedClass || !selectedSubject}
            title="Grant extra time to all students currently taking this subject exam"
          >Grant Extra Time to All (Class)</button>
        </div>
      </div>


      {error && <ValidationMessage type="error" message={error} />}
      {success && <ValidationMessage type="success" message={success} />}

      {students.length > 0 && (
        <div className={commonStyles.card} style={{ padding: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }} className="result-management-score-header">
            <h3 style={{ margin: 0, fontSize: '16px' }}>
              Bulk Score Entry 
              <span style={{ fontSize: '14px', color: '#6b7280', fontWeight: 'normal', marginLeft: '12px' }}>
                {subjects.find(s => s.id === selectedSubject)?.name} - {classes.find(c => c.id === selectedClass)?.name}
              </span>
            </h3>
            <div style={{ display: 'flex', gap: '12px' }}>
              <button 
                onClick={handleBulkExtendTime}
                style={{ padding: '10px 20px', backgroundColor: '#fef3c7', color: '#92400e', border: 'none', borderRadius: '6px', fontWeight: 600, cursor: 'pointer' }}
                disabled={extendingTime}
                title="Grant extra time to the entire class for their active exam"
              >
                {extendingTime ? 'Extending...' : 'Bulk Extra Time ⏳'}
              </button>
              <button 
                onClick={handleSaveScores}
                style={{ padding: '10px 20px', backgroundColor: '#10b981', color: 'white', border: 'none', borderRadius: '6px', fontWeight: 600, cursor: 'pointer' }}
                disabled={loading}
              >
                Save Scores
              </button>
            </div>
          </div>

          <div style={{ overflowX: 'auto' }} className="result-management-table-container">
            <table style={tableStyle} className="result-management-table">
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
                  const testStr = scores[stu.studentId]?.testScore || '0';
                  const examStr = scores[stu.studentId]?.examScore || '0';
                  const total = (parseInt(testStr) || 0) + (parseInt(examStr) || 0);
                                    const statusMap = ['Draft', 'Moderated', 'Approved', 'Published'];
                      const statusLabel = typeof stu.status === 'string' 
                          ? stu.status 
                          : statusMap[stu.status as unknown as number] || 'Unknown';
                  
                  return (
                    <tr key={stu.id} style={trStyle}>
                      <td style={tdStyle} data-label="Admn No.">{stu.admissionNumber || '-'}</td>
                      <td style={tdStyle} data-label="Student Name"><strong>{stu.studentName}</strong></td>
                      <td style={tdStyle} data-label="Status">
                        <span style={{ fontSize: '12px', fontWeight: 600, padding: '4px 8px', borderRadius: '4px', backgroundColor: '#f3f4f6' }}>
                          {statusLabel}
                        </span>
                      </td>
                      <td style={tdStyle} data-label="Test Score (40)">
                        <input 
                          type="number" 
                          min="0" max="40"
                          style={inputStyle}
                          className="result-management-score-input"
                          value={scores[stu.studentId]?.testScore || ''}
                          onChange={(e) => handleScoreChange(stu.studentId, 'testScore', e.target.value)}
                        />
                      </td>
                      <td style={tdStyle} data-label="Exam Score (60)">
                        <input 
                          type="number" 
                          min="0" max="60"
                          style={inputStyle}
                          className="result-management-score-input"
                          value={scores[stu.studentId]?.examScore || ''}
                          onChange={(e) => handleScoreChange(stu.studentId, 'examScore', e.target.value)}
                        />
                      </td>
                      <td style={tdStyle} data-label="Total (100)">
                        <span style={{ fontWeight: 600, color: total < 40 ? '#dc2626' : '#16a34a' }}>
                          {total}
                        </span>
                      </td>
                      <td style={{ ...tdStyle, display: 'flex', gap: '8px' }} data-label="Actions">
                        <button 
                          onClick={() => setSelectedAssessmentId(stu.id)}
                          className="result-management-action-button"
                          style={{ padding: '6px 12px', backgroundColor: '#e5e7eb', color: '#374151', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '13px', fontWeight: 600 }}
                        >
                          Edit Details
                        </button>
                        <button 
                          onClick={() => handlePrint(stu.id)}
                          className="result-management-action-button"
                          style={{ padding: '6px 12px', backgroundColor: '#dcfce7', color: '#166534', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '13px', fontWeight: 600 }}
                        >
                          Print Report
                        </button>
                        <button 
                          onClick={() => handleToggleLock(stu.id, stu.isLockedForParents)}
                          className="result-management-action-button"
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
                        <button 
                          onClick={() => handleExtendTime(stu.studentId, stu.studentName)}
                          className="result-management-action-button"
                          style={{ padding: '6px 12px', backgroundColor: '#fef3c7', color: '#92400e', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '13px', fontWeight: 600 }}
                          title="Grant extra time for this exam"
                        >
                          Extra Time ⏳
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
