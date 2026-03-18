import React, { useEffect, useState, useCallback } from 'react';
import { SubjectDto, SubjectCreateDto } from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface SubjectFormModalProps {
  isOpen: boolean;
  onConfirm: (subject: SubjectCreateDto) => void;
  onCancel: () => void;
  apiError?: string;
}

const SubjectFormModal = ({ isOpen, onConfirm, onCancel, apiError }: SubjectFormModalProps) => {
  const [formData, setFormData] = useState<SubjectCreateDto>({
    name: '',
    code: '',
    displayOrder: 0,
  });
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      setFormData({ name: '', code: '', displayOrder: 0 });
      setError('');
    }
  }, [isOpen]);

  useEffect(() => {
    if (apiError) setError(apiError);
  }, [apiError]);

  const validate = () => {
    if (!formData.name.trim()) {
      setError('Subject Name is required');
      return false;
    }
    return true;
  };

  const handleConfirm = () => {
    if (validate()) {
      onConfirm(formData);
    }
  };

  if (!isOpen) return null;

  return (
    <div style={styles.modalOverlay}>
      <div style={styles.modal}>
        <h3 style={styles.modalTitle}>Create New Subject</h3>
        {error && <ValidationMessage type="error" message={error} />}
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>Subject Name:</label>
          <input
            style={styles.formInput}
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            placeholder="e.g. Mathematics"
          />
        </div>
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>Subject Code (optional):</label>
          <input
            style={styles.formInput}
            value={formData.code || ''}
            onChange={(e) => setFormData({ ...formData, code: e.target.value })}
            placeholder="e.g. MTH101"
          />
        </div>
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>Display Order:</label>
          <input
            type="number"
            style={styles.formInput}
            value={formData.displayOrder}
            onChange={(e) => setFormData({ ...formData, displayOrder: parseInt(e.target.value) || 0 })}
          />
        </div>

        <div style={styles.modalActions}>
          <button onClick={onCancel} style={styles.modalCancelButton}>Cancel</button>
          <button onClick={handleConfirm} style={styles.modalConfirmButton}>Create Subject</button>
        </div>
      </div>
    </div>
  );
};

const SubjectManagement = () => {
  const [subjects, setSubjects] = useState<SubjectDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [formModal, setFormModal] = useState({ isOpen: false, apiError: '' });

  const loadSubjects = useCallback(async () => {
    setLoading(true);
    try {
      const response = await academicStructureService.getSubjects();
      if (response.isSuccess) {
        setSubjects(response.data || []);
      }
    } catch (err: any) {
      setError('Failed to load subjects: ' + err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadSubjects();
  }, [loadSubjects]);

  const handleCreate = async (data: SubjectCreateDto) => {
    try {
      const resp = await academicStructureService.createSubject(data);
      if (resp.isSuccess) {
        setFormModal({ isOpen: false, apiError: '' });
        loadSubjects();
      } else {
        setFormModal({ isOpen: true, apiError: resp.message || 'Failed to create subject' });
      }
    } catch (err: any) {
      setFormModal({ isOpen: true, apiError: err.message || 'Error occurred' });
    }
  };

  return (
    <div>
      <SubjectFormModal 
        isOpen={formModal.isOpen} 
        onConfirm={handleCreate} 
        onCancel={() => setFormModal({ isOpen: false, apiError: '' })} 
        apiError={formModal.apiError}
      />
      
      <div style={styles.header}>
        <h3 style={{ margin: 0 }}>Subject Management</h3>
        <button style={styles.createButton} onClick={() => setFormModal({ isOpen: true, apiError: '' })}>
          Create Subject
        </button>
      </div>
      
      {error && <ValidationMessage type="error" message={error} />}
      
      {loading ? <p>Loading subjects...</p> : (
        <div style={styles.tableContainer}>
          <table style={styles.table}>
            <thead>
              <tr>
                <th style={styles.th}>Name</th>
                <th style={styles.th}>Code</th>
                <th style={styles.th}>Display Order</th>
              </tr>
            </thead>
            <tbody>
              {subjects.length === 0 ? (
                <tr><td colSpan={3} style={styles.td}>No subjects found.</td></tr>
              ) : (
                subjects.map(s => (
                  <tr key={s.id} style={styles.tr}>
                    <td style={styles.td}><strong>{s.name}</strong></td>
                    <td style={styles.td}>{s.code || '-'}</td>
                    <td style={styles.td}>{s.displayOrder}</td>
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

export default SubjectManagement;
