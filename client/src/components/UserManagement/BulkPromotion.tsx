import React, { useState, useEffect } from 'react';
import { User } from '../../models/user';
import { SessionDto, TermDto, ClassLevelDto } from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { userService } from '../../services/userService';
import { getNextClassName } from '../ResultManagement/PrintableReportCard';

const BulkPromotion: React.FC = () => {
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [sourceTerms, setSourceTerms] = useState<TermDto[]>([]);
  const [targetTerms, setTargetTerms] = useState<TermDto[]>([]);
  const [classLevels, setClassLevels] = useState<ClassLevelDto[]>([]);

  // Source selection
  const [sourceSessionId, setSourceSessionId] = useState<string>('');
  const [sourceTermId, setSourceTermId] = useState<string>('');
  const [sourceClassLevelId, setSourceClassLevelId] = useState<string>('');

  // Target selection
  const [targetSessionId, setTargetSessionId] = useState<string>('');
  const [targetTermId, setTargetTermId] = useState<string>('');
  const [targetClassLevelId, setTargetClassLevelId] = useState<string>('');

  const [students, setStudents] = useState<User[]>([]);
  const [selectedStudentIds, setSelectedStudentIds] = useState<Set<string>>(new Set());
  
  const [loading, setLoading] = useState<boolean>(false);
  const [promoting, setPromoting] = useState<boolean>(false);
  const [error, setError] = useState<string>('');
  const [success, setSuccess] = useState<string>('');

  useEffect(() => {
    fetchAcademicData();
  }, []);

  const fetchAcademicData = async () => {
    try {
      const [sessRes, classRes] = await Promise.all([
        academicStructureService.getSessions(),
        academicStructureService.getClassLevels()
      ]);
      setSessions(sessRes.data || []);
      setClassLevels(classRes.data || []);
    } catch (err: any) {
      setError('Failed to load academic structure data.');
    }
  };

  // When source session changes, load terms and auto-select Third Term (promotion term)
  useEffect(() => {
    if (sourceSessionId) {
      academicStructureService.getTermsBySession(sourceSessionId).then(res => {
        const terms = res.data || [];
        setSourceTerms(terms);
        // Find third term or cumulative term
        const thirdTerm = terms.find(t => 
          t.name.toLowerCase().includes('third') || 
          t.name.toLowerCase().includes('3rd') || 
          t.isCumulativeTerm
        );
        if (thirdTerm) {
          setSourceTermId(thirdTerm.id);
        } else if (terms.length > 0) {
          setSourceTermId(terms[terms.length - 1].id);
        }
      });
    } else {
      setSourceTerms([]);
      setSourceTermId('');
    }
  }, [sourceSessionId]);

  // When target session changes, load terms and auto-select First Term (start of new session)
  useEffect(() => {
    if (targetSessionId) {
      academicStructureService.getTermsBySession(targetSessionId).then(res => {
        const terms = res.data || [];
        setTargetTerms(terms);
        // Find first term
        const firstTerm = terms.find(t => 
          t.name.toLowerCase().includes('first') || 
          t.name.toLowerCase().includes('1st')
        );
        if (firstTerm) {
          setTargetTermId(firstTerm.id);
        } else if (terms.length > 0) {
          setTargetTermId(terms[0].id);
        }
      });
    } else {
      setTargetTerms([]);
      setTargetTermId('');
    }
  }, [targetSessionId]);

  // When source class changes, auto-suggest the next class in target class
  const handleSourceClassChange = (selectedClassId: string) => {
    setSourceClassLevelId(selectedClassId);
    if (!selectedClassId) return;

    const currentClass = classLevels.find(c => c.id === selectedClassId);
    if (currentClass) {
      const predictedNextName = getNextClassName(currentClass.name);
      if (predictedNextName && predictedNextName !== 'the next class' && predictedNextName !== 'Graduation') {
        const matchingTargetClass = classLevels.find(
          c => c.name.trim().toLowerCase() === predictedNextName.trim().toLowerCase()
        );
        if (matchingTargetClass) {
          setTargetClassLevelId(matchingTargetClass.id);
        }
      }
    }
  };

  const fetchStudents = async () => {
    if (!sourceSessionId || !sourceClassLevelId) {
      setError('Please select Source Session and Class Level.');
      return;
    }
    setLoading(true);
    setError('');
    setSuccess('');
    try {
      const res = await userService.getStudentsByClass(sourceSessionId, sourceTermId || '', sourceClassLevelId);
      const studentList = res.data || [];
      setStudents(studentList);
      // Auto select all
      const allIds = new Set(studentList.map((s: User) => s.id));
      setSelectedStudentIds(allIds);
      if (studentList.length === 0) {
        setError('No students found for the selected session and class.');
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to fetch students.');
    } finally {
      setLoading(false);
    }
  };

  const handleSelectAll = (checked: boolean) => {
    if (checked) {
      setSelectedStudentIds(new Set(students.map(s => s.id)));
    } else {
      setSelectedStudentIds(new Set());
    }
  };

  const handleSelectStudent = (id: string, checked: boolean) => {
    const newSelected = new Set(selectedStudentIds);
    if (checked) {
      newSelected.add(id);
    } else {
      newSelected.delete(id);
    }
    setSelectedStudentIds(newSelected);
  };

  const handlePromote = async () => {
    if (!targetSessionId || !targetTermId || !targetClassLevelId) {
      setError('Please select Target Session, Term (First Term), and Class Level.');
      return;
    }
    if (selectedStudentIds.size === 0) {
      setError('Please select at least one student to promote.');
      return;
    }

    setPromoting(true);
    setError('');
    setSuccess('');

    try {
      await userService.bulkEnrollStudents({
        studentIds: Array.from(selectedStudentIds),
        sessionId: targetSessionId,
        termId: targetTermId,
        classLevelId: targetClassLevelId
      });
      setSuccess(`Successfully promoted ${selectedStudentIds.size} student(s)!`);
      setStudents([]);
      setSelectedStudentIds(new Set());
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to promote students.');
    } finally {
      setPromoting(false);
    }
  };

  return (
    <div className="bulk-promotion-container" style={{ padding: '20px', maxWidth: '1000px', margin: '0 auto' }}>
      <h2>Bulk Promotion</h2>
      <p style={{ color: '#555', marginBottom: '20px' }}>
        Promote students from the current academic session (Third Term) into the next session and class (First Term).
      </p>

      {error && <div style={{ color: '#d32f2f', backgroundColor: '#ffebee', padding: '10px 14px', borderRadius: '4px', marginBottom: '16px' }}>{error}</div>}
      {success && <div style={{ color: '#2e7d32', backgroundColor: '#e8f5e9', padding: '10px 14px', borderRadius: '4px', marginBottom: '16px' }}>{success}</div>}

      <div style={{ display: 'flex', gap: '20px', marginBottom: '20px' }}>
        {/* SOURCE */}
        <div style={{ flex: 1, border: '1px solid #ddd', padding: '18px', borderRadius: '8px', backgroundColor: '#fafafa' }}>
          <h3 style={{ marginTop: 0, marginBottom: '14px', borderBottom: '1px solid #eee', paddingBottom: '8px' }}>
            1. Source Class (Current Session)
          </h3>
          <div style={{ marginBottom: '12px' }}>
            <label style={{ display: 'block', fontWeight: 600, marginBottom: '4px' }}>Session</label>
            <select 
              value={sourceSessionId} 
              onChange={e => setSourceSessionId(e.target.value)} 
              style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }}
            >
              <option value="">Select Session</option>
              {sessions.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
            </select>
          </div>
          <div style={{ marginBottom: '12px' }}>
            <label style={{ display: 'block', fontWeight: 600, marginBottom: '4px' }}>
              Term <span style={{ fontWeight: 'normal', color: '#666', fontSize: '0.85rem' }}>(Default: Third Term)</span>
            </label>
            <select 
              value={sourceTermId} 
              onChange={e => setSourceTermId(e.target.value)} 
              style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }}
            >
              <option value="">All Terms / Third Term</option>
              {sourceTerms.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
            </select>
          </div>
          <div style={{ marginBottom: '16px' }}>
            <label style={{ display: 'block', fontWeight: 600, marginBottom: '4px' }}>Class Level</label>
            <select 
              value={sourceClassLevelId} 
              onChange={e => handleSourceClassChange(e.target.value)} 
              style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }}
            >
              <option value="">Select Class</option>
              {classLevels.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </div>
          <button 
            onClick={fetchStudents} 
            disabled={loading || !sourceSessionId || !sourceClassLevelId} 
            style={{ 
              padding: '10px 18px', 
              cursor: loading || !sourceSessionId || !sourceClassLevelId ? 'not-allowed' : 'pointer', 
              backgroundColor: '#1976d2', 
              color: 'white', 
              border: 'none', 
              borderRadius: '4px',
              fontWeight: 600,
              opacity: loading || !sourceSessionId || !sourceClassLevelId ? 0.6 : 1
            }}
          >
            {loading ? 'Fetching Students...' : 'Fetch Students'}
          </button>
        </div>

        {/* TARGET */}
        <div style={{ flex: 1, border: '1px solid #ddd', padding: '18px', borderRadius: '8px', backgroundColor: '#fafafa' }}>
          <h3 style={{ marginTop: 0, marginBottom: '14px', borderBottom: '1px solid #eee', paddingBottom: '8px' }}>
            3. Target Class (New Session)
          </h3>
          <div style={{ marginBottom: '12px' }}>
            <label style={{ display: 'block', fontWeight: 600, marginBottom: '4px' }}>Target Session</label>
            <select 
              value={targetSessionId} 
              onChange={e => setTargetSessionId(e.target.value)} 
              style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }}
            >
              <option value="">Select Next Session</option>
              {sessions.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
            </select>
          </div>
          <div style={{ marginBottom: '12px' }}>
            <label style={{ display: 'block', fontWeight: 600, marginBottom: '4px' }}>
              Target Term <span style={{ fontWeight: 'normal', color: '#666', fontSize: '0.85rem' }}>(Default: First Term)</span>
            </label>
            <select 
              value={targetTermId} 
              onChange={e => setTargetTermId(e.target.value)} 
              style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }}
            >
              <option value="">Select Term</option>
              {targetTerms.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
            </select>
          </div>
          <div style={{ marginBottom: '16px' }}>
            <label style={{ display: 'block', fontWeight: 600, marginBottom: '4px' }}>Target Class Level</label>
            <select 
              value={targetClassLevelId} 
              onChange={e => setTargetClassLevelId(e.target.value)} 
              style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }}
            >
              <option value="">Select Next Class</option>
              {classLevels.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </div>
          <button 
            onClick={handlePromote} 
            disabled={promoting || selectedStudentIds.size === 0 || !targetSessionId || !targetClassLevelId || !targetTermId} 
            style={{ 
              padding: '10px 18px', 
              cursor: promoting || selectedStudentIds.size === 0 ? 'not-allowed' : 'pointer', 
              backgroundColor: '#388e3c', 
              color: 'white', 
              border: 'none', 
              borderRadius: '4px',
              fontWeight: 600,
              opacity: promoting || selectedStudentIds.size === 0 || !targetSessionId || !targetClassLevelId || !targetTermId ? 0.6 : 1
            }}
          >
            {promoting ? 'Promoting...' : `Promote Selected (${selectedStudentIds.size})`}
          </button>
        </div>
      </div>

      {/* STUDENTS LIST */}
      <div style={{ border: '1px solid #ddd', padding: '18px', borderRadius: '8px', backgroundColor: '#fff' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px' }}>
          <h3 style={{ margin: 0 }}>2. Select Students ({selectedStudentIds.size} of {students.length} selected)</h3>
          {students.length > 0 && (
            <div>
              <button 
                type="button" 
                onClick={() => handleSelectAll(true)} 
                style={{ marginRight: '8px', padding: '5px 12px', fontSize: '0.85rem', cursor: 'pointer' }}
              >
                Select All
              </button>
              <button 
                type="button" 
                onClick={() => handleSelectAll(false)} 
                style={{ padding: '5px 12px', fontSize: '0.85rem', cursor: 'pointer' }}
              >
                Deselect All
              </button>
            </div>
          )}
        </div>

        {students.length > 0 ? (
          <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: '10px' }}>
            <thead>
              <tr style={{ backgroundColor: '#f2f2f2' }}>
                <th style={{ padding: '10px', border: '1px solid #ddd', textAlign: 'center', width: '50px' }}>
                  <input 
                    type="checkbox" 
                    checked={selectedStudentIds.size === students.length && students.length > 0} 
                    onChange={e => handleSelectAll(e.target.checked)} 
                  />
                </th>
                <th style={{ padding: '10px', border: '1px solid #ddd', textAlign: 'left', width: '200px' }}>Admission No</th>
                <th style={{ padding: '10px', border: '1px solid #ddd', textAlign: 'left' }}>Student Name</th>
              </tr>
            </thead>
            <tbody>
              {students.map(student => (
                <tr 
                  key={student.id} 
                  style={{ 
                    backgroundColor: selectedStudentIds.has(student.id) ? '#f0f7ff' : 'white',
                    cursor: 'pointer' 
                  }}
                  onClick={() => handleSelectStudent(student.id, !selectedStudentIds.has(student.id))}
                >
                  <td style={{ padding: '10px', border: '1px solid #ddd', textAlign: 'center' }} onClick={e => e.stopPropagation()}>
                    <input 
                      type="checkbox" 
                      checked={selectedStudentIds.has(student.id)} 
                      onChange={e => handleSelectStudent(student.id, e.target.checked)} 
                    />
                  </td>
                  <td style={{ padding: '10px', border: '1px solid #ddd', fontWeight: 500 }}>{student.admissionNumber || '-'}</td>
                  <td style={{ padding: '10px', border: '1px solid #ddd' }}>{student.fullName}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p style={{ color: '#777', fontStyle: 'italic', margin: '15px 0' }}>
            No students loaded yet. Select a Source Session and Class Level above, then click &quot;Fetch Students&quot;.
          </p>
        )}
      </div>
    </div>
  );
};

export default BulkPromotion;
