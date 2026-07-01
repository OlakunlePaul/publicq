import React, { useState, useEffect } from 'react';
import { User } from '../../models/user';
import { userService } from '../../services/userService';
import { academicStructureService } from '../../services/academicStructureService';
import { SessionDto, TermDto } from '../../models/academic';
import userManagementStyles from './UserManagement.module.css';

interface ManageEnrollmentsModalProps {
  isOpen: boolean;
  user: User | null;
  onConfirm: () => void;
  onCancel: () => void;
}

const ManageEnrollmentsModal: React.FC<ManageEnrollmentsModalProps> = ({
  isOpen,
  user,
  onConfirm,
  onCancel
}) => {
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [terms, setTerms] = useState<TermDto[]>([]);
  
  const [selectedSessionId, setSelectedSessionId] = useState('');
  const [selectedTermId, setSelectedTermId] = useState('');
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');

  useEffect(() => {
    if (isOpen) {
      loadInitialData();
      setSelectedSessionId('');
      setSelectedTermId('');
      setError('');
      setSuccessMessage('');
    }
  }, [isOpen]);

  useEffect(() => {
    if (selectedSessionId) {
      loadTerms(selectedSessionId);
    } else {
      setTerms([]);
      setSelectedTermId('');
    }
  }, [selectedSessionId]);

  const loadInitialData = async () => {
    try {
      const sessionsRes = await academicStructureService.getSessions();
      setSessions(sessionsRes.data || []);
    } catch (err) {
      setError('Failed to load academic structure data.');
    }
  };

  const loadTerms = async (sessionId: string) => {
    try {
      const response = await academicStructureService.getTermsBySession(sessionId);
      setTerms(response.data || []);
    } catch (err) {
      setError('Failed to load terms.');
    }
  };

  const handleEnroll = async () => {
    if (!user) return;
    if (!selectedSessionId || !selectedTermId) {
      setError('Please select Session and Term.');
      return;
    }

    setLoading(true);
    setError('');
    setSuccessMessage('');

    try {
      await userService.addStudentEnrollment(user.id, {
        sessionId: selectedSessionId,
        termId: selectedTermId
      });
      setSuccessMessage('Enrollment added successfully!');
      setTimeout(() => {
        onConfirm();
      }, 1500);
    } catch (err: any) {
      setError('Failed to add enrollment: ' + (err.response?.data?.message || err.message));
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen || !user) return null;

  return (
    <div className={userManagementStyles.modalOverlay}>
      <div className={userManagementStyles.modal}>
        <div className={userManagementStyles.modalHeader}>
          <h3 className={userManagementStyles.modalTitle}>Manage Enrollments</h3>
          <p className={userManagementStyles.modalSubtitle}>
            Enrolling student: <strong>{user.fullName || user.id}</strong>
          </p>
        </div>
        
        <div className={userManagementStyles.modalBody}>
          {error && (
            <div className={userManagementStyles.errorBox}>
              <p className={userManagementStyles.errorMessage}>{error}</p>
            </div>
          )}
          {successMessage && (
            <div style={{ backgroundColor: '#d1fae5', color: '#065f46', padding: '10px', borderRadius: '4px', marginBottom: '15px' }}>
              <p style={{ margin: 0 }}>{successMessage}</p>
            </div>
          )}
          
          <div className={userManagementStyles.infoBox}>
            <p className={userManagementStyles.modalInfoText}>
              Enrolling a student in a new term will allow you to upload scores for them in that term. This will not delete their existing term data.
            </p>
          </div>
          
          <div className={userManagementStyles.formContainer}>
            <div className={userManagementStyles.formGroup}>
              <label className={userManagementStyles.formLabel}>Session: <span style={{color: '#dc2626'}}>*</span></label>
              <select
                value={selectedSessionId}
                onChange={(e) => setSelectedSessionId(e.target.value)}
                className={userManagementStyles.formInput}
                style={{ backgroundColor: 'white' }}
                disabled={loading}
              >
                <option value="">Select Session</option>
                {sessions.map(s => (
                  <option key={s.id} value={s.id}>{s.name}</option>
                ))}
              </select>
            </div>

            <div className={userManagementStyles.formGroup}>
              <label className={userManagementStyles.formLabel}>Term: <span style={{color: '#dc2626'}}>*</span></label>
              <select
                value={selectedTermId}
                onChange={(e) => setSelectedTermId(e.target.value)}
                className={userManagementStyles.formInput}
                style={{ backgroundColor: 'white' }}
                disabled={!selectedSessionId || loading}
              >
                <option value="">Select Term</option>
                {terms.map(t => (
                  <option key={t.id} value={t.id}>{t.name}</option>
                ))}
              </select>
            </div>
          </div>
        </div>
        
        <div className={userManagementStyles.modalFooter}>
          <button 
            onClick={onCancel} 
            disabled={loading}
            className={userManagementStyles.modalCancelButton}
          >
            Cancel
          </button>
          <button 
            onClick={handleEnroll} 
            disabled={loading || !selectedSessionId || !selectedTermId}
            className={userManagementStyles.modalConfirmButton}
          >
            {loading ? 'Enrolling...' : 'Add Enrollment'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ManageEnrollmentsModal;
