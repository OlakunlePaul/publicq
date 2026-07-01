import React, { useState, useEffect } from 'react';
import { User } from '../../models/user';
import { SessionDto, TermDto, ClassLevelDto } from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { userService } from '../../services/userService';


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

  useEffect(() => {
    if (sourceSessionId) {
      academicStructureService.getTermsBySession(sourceSessionId).then(res => setSourceTerms(res.data || []));
    } else {
      setSourceTerms([]);
      setSourceTermId('');
    }
  }, [sourceSessionId]);

  useEffect(() => {
    if (targetSessionId) {
      academicStructureService.getTermsBySession(targetSessionId).then(res => setTargetTerms(res.data || []));
    } else {
      setTargetTerms([]);
      setTargetTermId('');
    }
  }, [targetSessionId]);

  const fetchStudents = async () => {
    if (!sourceSessionId || !sourceTermId || !sourceClassLevelId) {
      setError('Please select Source Session, Term, and Class Level.');
      return;
    }
    setLoading(true);
    setError('');
    setSuccess('');
    try {
      const res = await userService.getStudentsByClass(sourceSessionId, sourceTermId, sourceClassLevelId);
      setStudents(res.data || []);
      // Auto select all
      const allIds = new Set((res.data || []).map((s: User) => s.id));
      setSelectedStudentIds(allIds);
    } catch (err: any) {
      setError('Failed to fetch students.');
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
      setError('Please select Target Session, Term, and Class Level.');
      return;
    }
    if (selectedStudentIds.size === 0) {
      setError('Please select at least one student.');
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
      setSuccess('Students promoted successfully!');
      // Clear selection or fetch students again? Probably keep it so they can see success message, but let's re-fetch to see if they want to move others.
      // Or just empty the students list.
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
      <p>Move students from a previous session/term/class to a new one.</p>

      {error && <div style={{ color: 'red', marginBottom: '10px' }}>{error}</div>}
      {success && <div style={{ color: 'green', marginBottom: '10px' }}>{success}</div>}

      <div style={{ display: 'flex', gap: '20px', marginBottom: '20px' }}>
        {/* SOURCE */}
        <div style={{ flex: 1, border: '1px solid #ddd', padding: '15px', borderRadius: '5px' }}>
          <h3>1. Source Class</h3>
          <div style={{ marginBottom: '10px' }}>
            <label style={{ display: 'block' }}>Session</label>
            <select value={sourceSessionId} onChange={e => setSourceSessionId(e.target.value)} style={{ width: '100%', padding: '5px' }}>
              <option value="">Select Session</option>
              {sessions.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
            </select>
          </div>
          <div style={{ marginBottom: '10px' }}>
            <label style={{ display: 'block' }}>Term</label>
            <select value={sourceTermId} onChange={e => setSourceTermId(e.target.value)} style={{ width: '100%', padding: '5px' }}>
              <option value="">Select Term</option>
              {sourceTerms.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
            </select>
          </div>
          <div style={{ marginBottom: '10px' }}>
            <label style={{ display: 'block' }}>Class Level</label>
            <select value={sourceClassLevelId} onChange={e => setSourceClassLevelId(e.target.value)} style={{ width: '100%', padding: '5px' }}>
              <option value="">Select Class</option>
              {classLevels.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </div>
          <button onClick={fetchStudents} disabled={loading} style={{ padding: '8px 15px', cursor: 'pointer' }}>
            {loading ? 'Loading...' : 'Fetch Students'}
          </button>
        </div>

        {/* TARGET */}
        <div style={{ flex: 1, border: '1px solid #ddd', padding: '15px', borderRadius: '5px' }}>
          <h3>3. Target Class</h3>
          <div style={{ marginBottom: '10px' }}>
            <label style={{ display: 'block' }}>Session</label>
            <select value={targetSessionId} onChange={e => setTargetSessionId(e.target.value)} style={{ width: '100%', padding: '5px' }}>
              <option value="">Select Session</option>
              {sessions.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
            </select>
          </div>
          <div style={{ marginBottom: '10px' }}>
            <label style={{ display: 'block' }}>Term</label>
            <select value={targetTermId} onChange={e => setTargetTermId(e.target.value)} style={{ width: '100%', padding: '5px' }}>
              <option value="">Select Term</option>
              {targetTerms.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
            </select>
          </div>
          <div style={{ marginBottom: '10px' }}>
            <label style={{ display: 'block' }}>Class Level</label>
            <select value={targetClassLevelId} onChange={e => setTargetClassLevelId(e.target.value)} style={{ width: '100%', padding: '5px' }}>
              <option value="">Select Class</option>
              {classLevels.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </div>
          <button onClick={handlePromote} disabled={promoting || students.length === 0} style={{ padding: '8px 15px', cursor: 'pointer', backgroundColor: '#4CAF50', color: 'white', border: 'none' }}>
            {promoting ? 'Promoting...' : 'Promote Selected'}
          </button>
        </div>
      </div>

      {/* STUDENTS LIST */}
      <div>
        <h3>2. Select Students</h3>
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
                <th style={{ padding: '10px', border: '1px solid #ddd', textAlign: 'left' }}>Admission No</th>
                <th style={{ padding: '10px', border: '1px solid #ddd', textAlign: 'left' }}>Name</th>
              </tr>
            </thead>
            <tbody>
              {students.map(student => (
                <tr key={student.id}>
                  <td style={{ padding: '10px', border: '1px solid #ddd', textAlign: 'center' }}>
                    <input 
                      type="checkbox" 
                      checked={selectedStudentIds.has(student.id)} 
                      onChange={e => handleSelectStudent(student.id, e.target.checked)} 
                    />
                  </td>
                  <td style={{ padding: '10px', border: '1px solid #ddd' }}>{student.admissionNumber || '-'}</td>
                  <td style={{ padding: '10px', border: '1px solid #ddd' }}>{student.fullName}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No students found for the selected source.</p>
        )}
      </div>
    </div>
  );
};

export default BulkPromotion;
