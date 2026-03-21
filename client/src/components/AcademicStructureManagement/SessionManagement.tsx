import React, { useEffect, useState, useCallback } from 'react';
import { SessionDto, SessionCreateDto } from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface SessionFormModalProps {
  isOpen: boolean;
  session?: SessionDto; // Pass session for editing
  onConfirm: (session: SessionCreateDto) => void;
  onCancel: () => void;
  apiError?: string;
}

const SessionFormModal = ({ isOpen, session, onConfirm, onCancel, apiError }: SessionFormModalProps) => {
  const [formData, setFormData] = useState<SessionCreateDto>({
    name: '',
    startDate: '',
    endDate: '',
    isActive: false,
  });
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      if (session) {
        setFormData({
          name: session.name,
          startDate: session.startDate ? session.startDate.split('T')[0] : '',
          endDate: session.endDate ? session.endDate.split('T')[0] : '',
          isActive: session.isActive,
        });
      } else {
        setFormData({ name: '', startDate: '', endDate: '', isActive: false });
      }
      setError('');
    }
  }, [isOpen, session]);

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
        <h3 style={styles.modalTitle}>{session ? 'Edit Session' : 'Create New Session'}</h3>
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
          <button onClick={handleConfirm} style={styles.modalConfirmButton}>{session ? 'Update Session' : 'Create Session'}</button>
        </div>
      </div>
    </div>
  );
};

const SessionManagement = () => {
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [formModal, setFormModal] = useState<{ isOpen: boolean; apiError: string; session?: SessionDto }>({ isOpen: false, apiError: '' });

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

  const handleSave = async (data: SessionCreateDto) => {
    try {
      let resp;
      if (formModal.session) {
        resp = await academicStructureService.updateSession(formModal.session.id, data);
      } else {
        resp = await academicStructureService.createSession(data);
      }

      if (resp.isSuccess) {
        setFormModal({ isOpen: false, apiError: '', session: undefined });
        loadSessions();
      } else {
        setFormModal({ ...formModal, apiError: resp.message || 'Failed to save session' });
      }
    } catch (err: any) {
      setFormModal({ ...formModal, apiError: err.message || 'Error occurred' });
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm("Are you sure you want to delete this session?")) return;
    try {
      const resp = await academicStructureService.deleteSession(id);
      if (resp.isSuccess) {
        loadSessions();
      } else {
        alert(resp.message || "Failed to delete session (ensure it has no terms)");
      }
    } catch (err: any) {
      alert("Error: " + err.message);
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

  // Add mobile responsive styles
  useEffect(() => {
    const style = document.createElement('style');
    style.textContent = `
      @media (max-width: 768px) {
        .session-management-header {
          flex-direction: column !important;
          align-items: stretch !important;
          gap: 16px !important;
          margin-bottom: 24px !important;
        }
        .session-management-header h3 {
          text-align: center !important;
        }
        .session-management-create-button {
          width: 100% !important;
          max-width: 300px !important;
          margin: 0 auto !important;
          height: 48px !important;
        }
        .session-management-table-container {
          border: none !important;
          box-shadow: none !important;
          background: transparent !important;
          overflow: visible !important;
        }
        .session-management-table, 
        .session-management-table thead, 
        .session-management-table tbody, 
        .session-management-table tr, 
        .session-management-table td {
          display: block !important;
          width: 100% !important;
        }
        .session-management-table thead {
          display: none !important;
        }
        .session-management-table tr {
          background-color: white !important;
          border-radius: 16px !important;
          margin-bottom: 20px !important;
          padding: 16px !important;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05), 0 1px 2px rgba(0, 0, 0, 0.1) !important;
          border: 1px solid #f3f4f6 !important;
        }
        .session-management-table td {
          border-bottom: 1px solid #f9fafb !important;
          padding: 12px 0 !important;
          display: flex !important;
          justify-content: space-between !important;
          align-items: center !important;
          text-align: right !important;
          min-height: 44px !important;
        }
        .session-management-table td:last-child {
          border-bottom: none !important;
          margin-top: 12px !important;
          flex-direction: column !important;
          align-items: stretch !important;
          text-align: left !important;
        }
        .session-management-table td::before {
          content: attr(data-label) !important;
          font-weight: 700 !important;
          color: #6b7280 !important;
          text-transform: uppercase !important;
          font-size: 11px !important;
          letter-spacing: 0.05em !important;
          text-align: left !important;
        }
        .session-management-action-button {
          height: 44px !important;
          margin-bottom: 8px !important;
          margin-right: 0 !important;
          display: flex !important;
          align-items: center !important;
          justify-content: center !important;
          width: 100% !important;
        }
      }
    `;
    document.head.appendChild(style);
    return () => {
      document.head.removeChild(style);
    };
  }, []);

  const formatDate = (dateString?: string) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div>
      <SessionFormModal 
        isOpen={formModal.isOpen} 
        session={formModal.session}
        onConfirm={handleSave} 
        onCancel={() => setFormModal({ isOpen: false, apiError: '', session: undefined })} 
        apiError={formModal.apiError}
      />
      
      <div style={styles.header} className="session-management-header">
        <h3 style={{ margin: 0 }}>Session Management</h3>
        <button 
          style={styles.createButton} 
          className="session-management-create-button"
          onClick={() => setFormModal({ isOpen: true, apiError: '', session: undefined })}
        >
          Create Session
        </button>
      </div>
      
      {error && <ValidationMessage type="error" message={error} />}
      
      {loading ? <p>Loading sessions...</p> : (
        <div style={styles.tableContainer} className="session-management-table-container">
          <table style={styles.table} className="session-management-table">
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
                    <td style={styles.td} data-label="Name"><strong>{s.name}</strong></td>
                    <td style={styles.td} data-label="Start Date">{formatDate(s.startDate)}</td>
                    <td style={styles.td} data-label="End Date">{formatDate(s.endDate)}</td>
                    <td style={styles.td} data-label="Status">
                      {s.isActive ? (
                        <span style={styles.activeBadge}>Active</span>
                      ) : (
                        <span style={styles.inactiveBadge}>Inactive</span>
                      )}
                    </td>
                    <td style={styles.td} data-label="Actions">
                      <button 
                        style={{ ...styles.actionButton, backgroundColor: '#3b82f6', marginRight: '8px' }} 
                        className="session-management-action-button"
                        onClick={() => setFormModal({ isOpen: true, apiError: '', session: s })}
                      >
                        Edit
                      </button>
                      {!s.isActive && (
                        <>
                          <button 
                            style={{ ...styles.actionButton, backgroundColor: '#10b981', marginRight: '8px' }} 
                            className="session-management-action-button"
                            onClick={() => setAsActive(s.id)}
                          >
                            Set Active
                          </button>
                          <button 
                            style={{ ...styles.actionButton, backgroundColor: '#ef4444' }} 
                            className="session-management-action-button"
                            onClick={() => handleDelete(s.id)}
                          >
                            Delete
                          </button>
                        </>
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
