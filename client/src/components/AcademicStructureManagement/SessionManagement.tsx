import React, { useEffect, useState, useCallback } from 'react';
import { SessionDto, SessionCreateDto } from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface SessionFormModalProps {
  isOpen: boolean;
  onConfirm: (session: SessionCreateDto) => void;
  onCancel: () => void;
  apiError?: string;
}

const SessionFormModal = ({ isOpen, onConfirm, onCancel, apiError }: SessionFormModalProps) => {
  const [formData, setFormData] = useState<SessionCreateDto>({
    name: '',
    startDate: '',
    endDate: '',
    isActive: false,
  });
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      setFormData({ name: '', startDate: '', endDate: '', isActive: false });
      setError('');
    }
  }, [isOpen]);

  useEffect(() => {
    if (apiError) setError(apiError);
  }, [apiError]);

  const validate = () => {
    if (!formData.name.trim()) {
      setError('Session Name is required');
      return false;
    }
    return true;
  };

  const handleConfirm = () => {
    if (validate()) {
      // Ensure empty strings are sent as undefined for optional dates to avoid API errors
      onConfirm({
        ...formData,
        startDate: formData.startDate ? formData.startDate : undefined,
        endDate: formData.endDate ? formData.endDate : undefined,
      });
    }
  };

  if (!isOpen) return null;

  return (
    <div style={styles.modalOverlay}>
      <div style={styles.modal}>
        <h3 style={styles.modalTitle}>Create New Session</h3>
        {error && <ValidationMessage type="error" message={error} />}
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>Session Name (e.g. 2024/2025):</label>
          <input
            style={styles.formInput}
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            placeholder="2024/2025"
          />
        </div>
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>Start Date (optional):</label>
          <input
            type="date"
            style={styles.formInput}
            value={formData.startDate || ''}
            onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
          />
        </div>
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>End Date (optional):</label>
          <input
            type="date"
            style={styles.formInput}
            value={formData.endDate || ''}
            onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
          />
        </div>

        <div style={styles.formGroup}>
          <label style={{ display: 'flex', alignItems: 'center', cursor: 'pointer', fontSize: '14px', fontWeight: 600 }}>
            <input
              type="checkbox"
              style={{ marginRight: '8px', cursor: 'pointer' }}
              checked={formData.isActive}
              onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
            />
            Set as Active Session
          </label>
          <p style={{ fontSize: '12px', color: '#6b7280', margin: '4px 0 0 20px' }}>
            Only one session can be active at a time. This will deactivate the current active session.
          </p>
        </div>

        <div style={styles.modalActions}>
          <button onClick={onCancel} style={styles.modalCancelButton}>Cancel</button>
          <button onClick={handleConfirm} style={styles.modalConfirmButton}>Create Session</button>
        </div>
      </div>
    </div>
  );
};

const SessionManagement = () => {
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [formModal, setFormModal] = useState({ isOpen: false, apiError: '' });

  const loadSessions = useCallback(async () => {
    setLoading(true);
    try {
      const response = await academicStructureService.getSessions();
      if (response.isSuccess) {
        setSessions(response.data || []);
      }
    } catch (err: any) {
      setError('Failed to load sessions: ' + err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadSessions();
  }, [loadSessions]);

  const handleCreate = async (data: SessionCreateDto) => {
    try {
      const resp = await academicStructureService.createSession(data);
      if (resp.isSuccess) {
        setFormModal({ isOpen: false, apiError: '' });
        loadSessions();
      } else {
        setFormModal({ isOpen: true, apiError: resp.message || 'Failed to create session' });
      }
    } catch (err: any) {
      setFormModal({ isOpen: true, apiError: err.message || 'Error occurred' });
    }
  };

  const setAsActive = async (sessionId: string) => {
    if (!window.confirm("Are you sure you want to make this the active session?")) return;
    try {
      setLoading(true);
      await academicStructureService.setActiveSession(sessionId);
      loadSessions();
    } catch (err: any) {
      alert("Error setting active session: " + err.message);
      setLoading(false);
    }
  }

  const formatDate = (dateString?: string) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div>
      <SessionFormModal 
        isOpen={formModal.isOpen} 
        onConfirm={handleCreate} 
        onCancel={() => setFormModal({ isOpen: false, apiError: '' })} 
        apiError={formModal.apiError}
      />
      
      <div style={styles.header}>
        <h3 style={{ margin: 0 }}>Session Management</h3>
        <button style={styles.createButton} onClick={() => setFormModal({ isOpen: true, apiError: '' })}>
          Create Session
        </button>
      </div>
      
      {error && <ValidationMessage type="error" message={error} />}
      
      {loading ? <p>Loading sessions...</p> : (
        <div style={styles.tableContainer}>
          <table style={styles.table}>
            <thead>
              <tr>
                <th style={styles.th}>Name</th>
                <th style={styles.th}>Start Date</th>
                <th style={styles.th}>End Date</th>
                <th style={styles.th}>Status</th>
                <th style={styles.th}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {sessions.length === 0 ? (
                <tr><td colSpan={5} style={styles.td}>No sessions found.</td></tr>
              ) : (
                sessions.map(s => (
                  <tr key={s.id} style={styles.tr}>
                    <td style={styles.td}><strong>{s.name}</strong></td>
                    <td style={styles.td}>{formatDate(s.startDate)}</td>
                    <td style={styles.td}>{formatDate(s.endDate)}</td>
                    <td style={styles.td}>
                      {s.isActive ? (
                        <span style={styles.activeBadge}>Active</span>
                      ) : (
                        <span style={styles.inactiveBadge}>Inactive</span>
                      )}
                    </td>
                    <td style={styles.td}>
                      {!s.isActive && (
                        <button style={styles.actionButton} onClick={() => setAsActive(s.id)}>Set Active</button>
                      )}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

const styles: Record<string, React.CSSProperties> = {
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '20px',
  },
  createButton: {
    padding: '10px 20px',
    backgroundColor: '#10b981',
    color: 'white',
    border: 'none',
    borderRadius: '6px',
    cursor: 'pointer',
    fontWeight: 'bold',
  },
  tableContainer: {
    overflowX: 'auto',
    borderRadius: '8px',
    border: '1px solid #e5e7eb',
    backgroundColor: '#ffffff',
  },
  table: {
    width: '100%',
    borderCollapse: 'collapse',
  },
  th: {
    textAlign: 'left',
    padding: '12px 16px',
    backgroundColor: '#f9fafb',
    borderBottom: '1px solid #e5e7eb',
    fontWeight: 600,
    fontSize: '14px',
    color: '#374151',
  },
  tr: {
    borderBottom: '1px solid #f3f4f6',
  },
  td: {
    padding: '12px 16px',
    fontSize: '14px',
    color: '#374151',
  },
  activeBadge: {
    backgroundColor: '#dcfce7', color: '#166534', padding: '4px 8px', borderRadius: '4px', fontSize: '12px', fontWeight: 600
  },
  inactiveBadge: {
    backgroundColor: '#f3f4f6', color: '#4b5563', padding: '4px 8px', borderRadius: '4px', fontSize: '12px', fontWeight: 600
  },
  actionButton: {
    padding: '6px 12px', backgroundColor: '#3b82f6', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '12px', fontWeight: 600
  },
  modalOverlay: {
    position: 'fixed',
    top: 0, left: 0, right: 0, bottom: 0,
    backgroundColor: 'rgba(0,0,0,0.5)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    zIndex: 1000,
  },
  modal: {
    backgroundColor: 'white',
    padding: '24px',
    borderRadius: '12px',
    width: '400px',
    maxWidth: '90%',
  },
  modalTitle: { margin: '0 0 16px 0', fontSize: '18px' },
  formGroup: { marginBottom: '16px' },
  formLabel: { display: 'block', marginBottom: '8px', fontSize: '14px', fontWeight: 600 },
  formInput: {
    width: '100%', padding: '10px', borderRadius: '6px',
    border: '1px solid #d1d5db', boxSizing: 'border-box'
  },
  modalActions: { display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '20px' },
  modalCancelButton: { padding: '8px 16px', borderRadius: '6px', border: '1px solid #d1d5db', cursor: 'pointer', backgroundColor: 'white' },
  modalConfirmButton: { padding: '8px 16px', borderRadius: '6px', border: 'none', backgroundColor: '#3b82f6', color: 'white', cursor: 'pointer' },
};

export default SessionManagement;
