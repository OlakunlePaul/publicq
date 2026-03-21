import React, { useEffect, useState, useCallback } from 'react';
import { TermDto, TermCreateDto, SessionDto } from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface TermFormModalProps {
  isOpen: boolean;
  term?: TermDto;
  sessionId: string;
  onConfirm: (term: TermCreateDto) => void;
  onCancel: () => void;
  apiError?: string;
}

const TermFormModal = ({ isOpen, term, sessionId, onConfirm, onCancel, apiError }: TermFormModalProps) => {
  const [formData, setFormData] = useState<TermCreateDto>({
    sessionId: sessionId,
    name: '',
    startDate: '',
    endDate: '',
    nextTermBegins: '',
    isActive: false,
  });
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      if (term) {
        setFormData({
          sessionId: term.sessionId,
          name: term.name,
          startDate: term.startDate ? term.startDate.split('T')[0] : '',
          endDate: term.endDate ? term.endDate.split('T')[0] : '',
          nextTermBegins: term.nextTermBegins ? term.nextTermBegins.split('T')[0] : '',
          isActive: term.isActive,
        });
      } else {
        setFormData({ sessionId, name: '', startDate: '', endDate: '', nextTermBegins: '', isActive: false });
      }
      setError('');
    }
  }, [isOpen, term, sessionId]);

  useEffect(() => {
    if (apiError) setError(apiError);
  }, [apiError]);

  const validate = () => {
    if (!formData.name.trim()) {
      setError('Term Name is required');
      return false;
    }
    return true;
  };

  const handleConfirm = () => {
    if (validate()) {
      onConfirm({
        ...formData,
        startDate: formData.startDate || undefined,
        endDate: formData.endDate || undefined,
        nextTermBegins: formData.nextTermBegins || undefined,
      });
    }
  };

  if (!isOpen) return null;

  return (
    <div style={styles.modalOverlay}>
      <div style={styles.modal}>
        <h3 style={styles.modalTitle}>{term ? 'Edit Term' : 'Create New Term'}</h3>
        {error && <ValidationMessage type="error" message={error} />}
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>Term Name (e.g. First Term):</label>
          <input
            style={styles.formInput}
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            placeholder="First Term"
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
          <label style={styles.formLabel}>Next Term Begins (optional):</label>
          <input
            type="date"
            style={styles.formInput}
            value={formData.nextTermBegins || ''}
            onChange={(e) => setFormData({ ...formData, nextTermBegins: e.target.value })}
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
            Set as Active Term
          </label>
        </div>

        <div style={styles.modalActions}>
          <button onClick={onCancel} style={styles.modalCancelButton}>Cancel</button>
          <button onClick={handleConfirm} style={styles.modalConfirmButton}>{term ? 'Update Term' : 'Create Term'}</button>
        </div>
      </div>
    </div>
  );
};

const TermManagement = () => {
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [selectedSessionId, setSelectedSessionId] = useState<string>('');
  const [terms, setTerms] = useState<TermDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [formModal, setFormModal] = useState<{ isOpen: boolean; apiError: string; term?: TermDto }>({ isOpen: false, apiError: '' });

  const loadSessions = useCallback(async () => {
    try {
      const response = await academicStructureService.getSessions();
      if (response.isSuccess && response.data) {
        setSessions(response.data);
        const active = response.data.find(s => s.isActive);
        if (active) setSelectedSessionId(active.id);
        else if (response.data.length > 0) setSelectedSessionId(response.data[0].id);
      }
    } catch (err: any) {
      setError('Failed to load sessions: ' + err.message);
    }
  }, []);

  const loadTerms = useCallback(async (sessionId: string) => {
    if (!sessionId) return;
    setLoading(true);
    try {
      const response = await academicStructureService.getTermsBySession(sessionId);
      if (response.isSuccess) {
        setTerms(response.data || []);
      }
    } catch (err: any) {
      setError('Failed to load terms: ' + err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadSessions();
  }, [loadSessions]);

  useEffect(() => {
    if (selectedSessionId) loadTerms(selectedSessionId);
  }, [selectedSessionId, loadTerms]);

  const handleSave = async (data: TermCreateDto) => {
    try {
      let resp;
      if (formModal.term) {
        resp = await academicStructureService.updateTerm(formModal.term.id, data);
      } else {
        resp = await academicStructureService.createTerm(data);
      }

      if (resp.isSuccess) {
        setFormModal({ isOpen: false, apiError: '', term: undefined });
        loadTerms(selectedSessionId);
      } else {
        setFormModal({ ...formModal, apiError: resp.message || 'Failed to save term' });
      }
    } catch (err: any) {
      setFormModal({ ...formModal, apiError: err.message || 'Error occurred' });
    }
  };

  // Add mobile responsive styles
  useEffect(() => {
    const style = document.createElement('style');
    style.textContent = `
      @media (max-width: 768px) {
        .term-management-header {
          flex-direction: column !important;
          align-items: stretch !important;
          gap: 16px !important;
          margin-bottom: 24px !important;
        }
        .term-management-header h3 {
          text-align: center !important;
        }
        .term-management-header-left {
          flex-direction: column !important;
          align-items: stretch !important;
        }
        .term-management-create-button {
          width: 100% !important;
          max-width: 300px !important;
          margin: 0 auto !important;
          height: 48px !important;
        }
        .term-management-table-container {
          border: none !important;
          box-shadow: none !important;
          background: transparent !important;
          overflow: visible !important;
        }
        .term-management-table, 
        .term-management-table thead, 
        .term-management-table tbody, 
        .term-management-table tr, 
        .term-management-table td {
          display: block !important;
          width: 100% !important;
        }
        .term-management-table thead {
          display: none !important;
        }
        .term-management-table tr {
          background-color: white !important;
          border-radius: 16px !important;
          margin-bottom: 20px !important;
          padding: 16px !important;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05), 0 1px 2px rgba(0, 0, 0, 0.1) !important;
          border: 1px solid #f3f4f6 !important;
        }
        .term-management-table td {
          border-bottom: 1px solid #f9fafb !important;
          padding: 12px 0 !important;
          display: flex !important;
          justify-content: space-between !important;
          align-items: center !important;
          text-align: right !important;
          min-height: 44px !important;
        }
        .term-management-table td:last-child {
          border-bottom: none !important;
          margin-top: 12px !important;
          flex-direction: column !important;
          align-items: stretch !important;
          text-align: left !important;
        }
        .term-management-table td::before {
          content: attr(data-label) !important;
          font-weight: 700 !important;
          color: #6b7280 !important;
          text-transform: uppercase !important;
          font-size: 11px !important;
          letter-spacing: 0.05em !important;
          text-align: left !important;
        }
        .term-management-action-button {
          height: 44px !important;
          margin-bottom: 8px !important;
          margin-right: 0 !important;
          display: flex !important;
          align-items: center !important;
          justify-content: center !important;
        }
      }
    `;
    document.head.appendChild(style);
    return () => {
      document.head.removeChild(style);
    };
  }, []);

  const handleDelete = async (id: string) => {
    if (!window.confirm("Are you sure you want to delete this term?")) return;
    try {
      const resp = await academicStructureService.deleteTerm(id);
      if (resp.isSuccess) {
        loadTerms(selectedSessionId);
      } else {
        alert(resp.message || "Failed to delete term");
      }
    } catch (err: any) {
      alert("Error: " + err.message);
    }
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div>
      <TermFormModal 
        isOpen={formModal.isOpen} 
        term={formModal.term}
        sessionId={selectedSessionId}
        onConfirm={handleSave} 
        onCancel={() => setFormModal({ isOpen: false, apiError: '', term: undefined })} 
        apiError={formModal.apiError}
      />
      
      <div style={styles.header} className="term-management-header">
        <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }} className="term-management-header-left">
          <h3 style={{ margin: 0 }}>Term Management</h3>
          <select 
            style={styles.select}
            value={selectedSessionId}
            onChange={(e) => setSelectedSessionId(e.target.value)}
          >
            {sessions.map(s => (
              <option key={s.id} value={s.id}>{s.name} {s.isActive ? '(Active)' : ''}</option>
            ))}
          </select>
        </div>
        <button 
          style={styles.createButton} 
          className="term-management-create-button"
          onClick={() => setFormModal({ isOpen: true, apiError: '', term: undefined })}
          disabled={!selectedSessionId}
        >
          Create Term
        </button>
      </div>
      
      {error && <ValidationMessage type="error" message={error} />}
      
      {loading ? <p>Loading terms...</p> : (
        <div style={styles.tableContainer} className="term-management-table-container">
          <table style={styles.table} className="term-management-table">
            <thead>
              <tr>
                <th style={styles.th}>Name</th>
                <th style={styles.th}>Start Date</th>
                <th style={styles.th}>End Date</th>
                <th style={styles.th}>Next Term Begins</th>
                <th style={styles.th}>Status</th>
                <th style={styles.th}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {terms.length === 0 ? (
                <tr><td colSpan={6} style={styles.td}>No terms found for this session.</td></tr>
              ) : (
                terms.map(t => (
                  <tr key={t.id} style={styles.tr}>
                    <td style={styles.td} data-label="Name"><strong>{t.name}</strong></td>
                    <td style={styles.td} data-label="Start Date">{formatDate(t.startDate)}</td>
                    <td style={styles.td} data-label="End Date">{formatDate(t.endDate)}</td>
                    <td style={styles.td} data-label="Next Term">{formatDate(t.nextTermBegins)}</td>
                    <td style={styles.td} data-label="Status">
                      {t.isActive ? (
                        <span style={styles.activeBadge}>Active</span>
                      ) : (
                        <span style={styles.inactiveBadge}>Inactive</span>
                      )}
                    </td>
                    <td style={styles.td} data-label="Actions">
                      <button 
                        style={{ ...styles.actionButton, backgroundColor: '#3b82f6', marginRight: '8px' }} 
                        className="term-management-action-button"
                        onClick={() => setFormModal({ isOpen: true, apiError: '', term: t })}
                      >
                        Edit
                      </button>
                      <button 
                        style={{ ...styles.actionButton, backgroundColor: '#ef4444' }} 
                        className="term-management-action-button"
                        onClick={() => handleDelete(t.id)}
                      >
                        Delete
                      </button>
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
  select: {
    padding: '8px 12px',
    borderRadius: '6px',
    border: '1px solid #d1d5db',
    fontSize: '14px',
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
    padding: '4px 8px',
    backgroundColor: '#dcfce7',
    color: '#166534',
    borderRadius: '9999px',
    fontSize: '12px',
    fontWeight: 600,
  },
  inactiveBadge: {
    padding: '4px 8px',
    backgroundColor: '#f3f4f6',
    color: '#374151',
    borderRadius: '9999px',
    fontSize: '12px',
    fontWeight: 600,
  },
  actionButton: {
    padding: '6px 12px',
    borderRadius: '4px',
    border: 'none',
    color: 'white',
    fontSize: '12px',
    fontWeight: 600,
    cursor: 'pointer',
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
    width: '450px',
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

export default TermManagement;
