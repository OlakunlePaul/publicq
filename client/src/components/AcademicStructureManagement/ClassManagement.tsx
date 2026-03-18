import React, { useEffect, useState, useCallback } from 'react';
import { ClassLevelDto, ClassLevelCreateDto } from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface ClassFormModalProps {
  isOpen: boolean;
  onConfirm: (classLevel: ClassLevelCreateDto) => void;
  onCancel: () => void;
  apiError?: string;
}

const ClassFormModal = ({ isOpen, onConfirm, onCancel, apiError }: ClassFormModalProps) => {
  const [formData, setFormData] = useState<ClassLevelCreateDto>({
    name: '',
    sectionOrArm: '',
    orderIndex: 0,
  });
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      setFormData({ name: '', sectionOrArm: '', orderIndex: 0 });
      setError('');
    }
  }, [isOpen]);

  useEffect(() => {
    if (apiError) setError(apiError);
  }, [apiError]);

  const validate = () => {
    if (!formData.name.trim()) {
      setError('Class Level Name is required');
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
        <h3 style={styles.modalTitle}>Create New Class</h3>
        {error && <ValidationMessage type="error" message={error} />}
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>Class Name:</label>
          <input
            style={styles.formInput}
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            placeholder="e.g. JSS 1"
          />
        </div>
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>Section / Arm (optional):</label>
          <input
            style={styles.formInput}
            value={formData.sectionOrArm || ''}
            onChange={(e) => setFormData({ ...formData, sectionOrArm: e.target.value })}
            placeholder="e.g. Science, Art, Gold"
          />
        </div>
        
        <div style={styles.formGroup}>
          <label style={styles.formLabel}>Display Order:</label>
          <input
            type="number"
            style={styles.formInput}
            value={formData.orderIndex}
            onChange={(e) => setFormData({ ...formData, orderIndex: parseInt(e.target.value) || 0 })}
            placeholder="Smaller numbers appear first"
          />
        </div>

        <div style={styles.modalActions}>
          <button onClick={onCancel} style={styles.modalCancelButton}>Cancel</button>
          <button onClick={handleConfirm} style={styles.modalConfirmButton}>Create Class</button>
        </div>
      </div>
    </div>
  );
};

const ClassManagement = () => {
  const [classes, setClasses] = useState<ClassLevelDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [formModal, setFormModal] = useState({ isOpen: false, apiError: '' });

  const loadClasses = useCallback(async () => {
    setLoading(true);
    try {
      const response = await academicStructureService.getClassLevels();
      if (response.isSuccess) {
        setClasses(response.data || []);
      }
    } catch (err: any) {
      setError('Failed to load classes: ' + err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadClasses();
  }, [loadClasses]);

  const handleCreate = async (data: ClassLevelCreateDto) => {
    try {
      const resp = await academicStructureService.createClassLevel(data);
      if (resp.isSuccess) {
        setFormModal({ isOpen: false, apiError: '' });
        loadClasses();
      } else {
        setFormModal({ isOpen: true, apiError: resp.message || 'Failed to create class' });
      }
    } catch (err: any) {
      setFormModal({ isOpen: true, apiError: err.message || 'Error occurred' });
    }
  };

  return (
    <div>
      <ClassFormModal 
        isOpen={formModal.isOpen} 
        onConfirm={handleCreate} 
        onCancel={() => setFormModal({ isOpen: false, apiError: '' })} 
        apiError={formModal.apiError}
      />
      
      <div style={styles.header}>
        <h3 style={{ margin: 0 }}>Class Management</h3>
        <button style={styles.createButton} onClick={() => setFormModal({ isOpen: true, apiError: '' })}>
          Create Class
        </button>
      </div>
      
      {error && <ValidationMessage type="error" message={error} />}
      
      {loading ? <p>Loading classes...</p> : (
        <div style={styles.tableContainer}>
          <table style={styles.table}>
            <thead>
              <tr>
                <th style={styles.th}>Name</th>
                <th style={styles.th}>Section/Arm</th>
                <th style={styles.th}>Display Order</th>
              </tr>
            </thead>
            <tbody>
              {classes.length === 0 ? (
                <tr><td colSpan={3} style={styles.td}>No classes found.</td></tr>
              ) : (
                classes.map(c => (
                  <tr key={c.id} style={styles.tr}>
                    <td style={styles.td}><strong>{c.name}</strong></td>
                    <td style={styles.td}>{c.sectionOrArm || '-'}</td>
                    <td style={styles.td}>{c.orderIndex}</td>
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

export default ClassManagement;
