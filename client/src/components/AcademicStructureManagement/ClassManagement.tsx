import React, { useEffect, useState, useCallback } from 'react';
import { ClassLevelDto, ClassLevelCreateDto, GradingSchemaDto, SubjectDto } from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface ClassFormModalProps {
  isOpen: boolean;
  classLevel?: ClassLevelDto; // Pass classLevel for editing
  schemas: GradingSchemaDto[];
  subjects: SubjectDto[];
  onConfirm: (classLevel: ClassLevelCreateDto) => void;
  onCancel: () => void;
  apiError?: string;
}

const ClassFormModal = ({ isOpen, classLevel, schemas, subjects, onConfirm, onCancel, apiError }: ClassFormModalProps) => {
  const [formData, setFormData] = useState<ClassLevelCreateDto>({
    name: '',
    sectionOrArm: '',
    orderIndex: 0,
    gradingSchemaId: undefined,
    subjectIds: []
  });
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      if (classLevel) {
        setFormData({ 
          name: classLevel.name, 
          sectionOrArm: classLevel.sectionOrArm || '', 
          orderIndex: classLevel.orderIndex,
          gradingSchemaId: classLevel.gradingSchemaId,
          subjectIds: classLevel.subjectIds || []
        });
      } else {
        setFormData({ name: '', sectionOrArm: '', orderIndex: 0, gradingSchemaId: undefined, subjectIds: [] });
      }
      setError('');
    }
  }, [isOpen, classLevel]);

  useEffect(() => {
    if (apiError) setError(apiError);
  }, [apiError]);

  const validate = () => {
    if (!formData.name.trim()) {
      setError('Class Name is required');
      return false;
    }
    return true;
  };

  const handleConfirm = () => {
    if (validate()) {
      onConfirm(formData);
    }
  };

  const toggleSubject = (subjectId: string) => {
    const currentIds = formData.subjectIds || [];
    if (currentIds.includes(subjectId)) {
      setFormData({ ...formData, subjectIds: currentIds.filter(id => id !== subjectId) });
    } else {
      setFormData({ ...formData, subjectIds: [...currentIds, subjectId] });
    }
  };

  if (!isOpen) return null;

  return (
    <div style={styles.modalOverlay}>
      <div style={{ ...styles.modal, width: '500px' }}>
        <h3 style={styles.modalTitle}>{classLevel ? 'Edit Class Level' : 'Create New Class Level'}</h3>
        {error && <ValidationMessage type="error" message={error} />}
        
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>
          <div>
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
              <label style={styles.formLabel}>Section/Arm:</label>
              <input
                style={styles.formInput}
                value={formData.sectionOrArm || ''}
                onChange={(e) => setFormData({ ...formData, sectionOrArm: e.target.value })}
                placeholder="e.g. A"
              />
            </div>

            <div style={styles.formGroup}>
              <label style={styles.formLabel}>Grading Schema:</label>
              <select
                style={styles.formInput}
                value={formData.gradingSchemaId || ''}
                onChange={(e) => setFormData({ ...formData, gradingSchemaId: e.target.value || undefined })}
              >
                <option value="">-- No Specific Schema --</option>
                {schemas.map(s => (
                  <option key={s.id} value={s.id}>{s.name}</option>
                ))}
              </select>
            </div>
            
            <div style={styles.formGroup}>
              <label style={styles.formLabel}>Sort Order:</label>
              <input
                type="number"
                style={styles.formInput}
                value={formData.orderIndex}
                onChange={(e) => setFormData({ ...formData, orderIndex: parseInt(e.target.value) || 0 })}
              />
            </div>
          </div>

          <div>
            <label style={styles.formLabel}>Link Subjects:</label>
            <div style={{ 
              maxHeight: '300px', 
              overflowY: 'auto', 
              border: '1px solid #d1d5db', 
              borderRadius: '6px',
              padding: '10px'
            }}>
              {subjects.length === 0 ? (
                <p style={{ fontSize: '12px', color: '#6b7280' }}>No subjects available. Create subjects first.</p>
              ) : (
                subjects.map(subject => (
                  <div key={subject.id} style={{ display: 'flex', alignItems: 'center', marginBottom: '8px' }}>
                    <input 
                      type="checkbox" 
                      id={`subject-${subject.id}`}
                      checked={formData.subjectIds?.includes(subject.id)}
                      onChange={() => toggleSubject(subject.id)}
                      style={{ marginRight: '8px' }}
                    />
                    <label htmlFor={`subject-${subject.id}`} style={{ fontSize: '14px', cursor: 'pointer' }}>
                      {subject.name}
                    </label>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>

        <div style={styles.modalActions}>
          <button onClick={onCancel} style={styles.modalCancelButton}>Cancel</button>
          <button onClick={handleConfirm} style={styles.modalConfirmButton}>{classLevel ? 'Update Class' : 'Create Class'}</button>
        </div>
      </div>
    </div>
  );
};

// Color palette for subject chips — 8 visually distinct pairs
const chipColors = [
  { bg: '#dbeafe', text: '#1e40af' },  // blue
  { bg: '#dcfce7', text: '#166534' },  // green
  { bg: '#fef3c7', text: '#92400e' },  // amber
  { bg: '#fce7f3', text: '#9d174d' },  // pink
  { bg: '#e0e7ff', text: '#3730a3' },  // indigo
  { bg: '#ccfbf1', text: '#115e59' },  // teal
  { bg: '#fee2e2', text: '#991b1b' },  // red
  { bg: '#f3e8ff', text: '#6b21a8' },  // purple
];

const ClassManagement = () => {
  const [classes, setClasses] = useState<ClassLevelDto[]>([]);
  const [schemas, setSchemas] = useState<GradingSchemaDto[]>([]);
  const [subjects, setSubjects] = useState<SubjectDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [formModal, setFormModal] = useState<{ isOpen: boolean; apiError: string; classLevel?: ClassLevelDto }>({ isOpen: false, apiError: '' });

  const loadClasses = useCallback(async () => {
    setLoading(true);
    try {
      const [classResp, schemaResp, subjectResp] = await Promise.all([
        academicStructureService.getClassLevels(),
        academicStructureService.getGradingSchemas(),
        academicStructureService.getSubjects()
      ]);

      if (classResp.isSuccess) {
        setClasses((classResp.data || []).sort((a, b) => a.orderIndex - b.orderIndex));
      }
      if (schemaResp.isSuccess) {
        setSchemas(schemaResp.data || []);
      }
      if (subjectResp.isSuccess) {
        setSubjects(subjectResp.data || []);
      }
    } catch (err: any) {
      setError('Failed to load classes, schemas or subjects: ' + err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadClasses();
  }, [loadClasses]);

  const handleSave = async (data: ClassLevelCreateDto) => {
    try {
      let resp;
      if (formModal.classLevel) {
        resp = await academicStructureService.updateClassLevel(formModal.classLevel.id, data);
      } else {
        resp = await academicStructureService.createClassLevel(data);
      }

      if (resp.isSuccess) {
        setFormModal({ isOpen: false, apiError: '', classLevel: undefined });
        loadClasses();
      } else {
        setFormModal({ ...formModal, apiError: resp.message || 'Failed to save class level' });
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
        .class-management-header {
          flex-direction: column !important;
          align-items: stretch !important;
          gap: 16px !important;
          margin-bottom: 24px !important;
        }
        .class-management-header h3 {
          text-align: center !important;
        }
        .class-management-create-button {
          width: 100% !important;
          max-width: 300px !important;
          margin: 0 auto !important;
          height: 48px !important;
        }
        .class-management-table-container {
          border: none !important;
          box-shadow: none !important;
          background: transparent !important;
          overflow: visible !important;
        }
        .class-management-table, 
        .class-management-table thead, 
        .class-management-table tbody, 
        .class-management-table tr, 
        .class-management-table td {
          display: block !important;
          width: 100% !important;
        }
        .class-management-table thead {
          display: none !important;
        }
        .class-management-table tr {
          background-color: white !important;
          border-radius: 16px !important;
          margin-bottom: 20px !important;
          padding: 16px !important;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05), 0 1px 2px rgba(0, 0, 0, 0.1) !important;
          border: 1px solid #f3f4f6 !important;
        }
        .class-management-table td {
          border-bottom: 1px solid #f9fafb !important;
          padding: 12px 0 !important;
          display: flex !important;
          justify-content: space-between !important;
          align-items: center !important;
          text-align: right !important;
          min-height: 44px !important;
        }
        .class-management-table td:last-child {
          border-bottom: none !important;
          margin-top: 12px !important;
          flex-direction: column !important;
          align-items: stretch !important;
          text-align: left !important;
        }
        .class-management-table td::before {
          content: attr(data-label) !important;
          font-weight: 700 !important;
          color: #6b7280 !important;
          text-transform: uppercase !important;
          font-size: 11px !important;
          letter-spacing: 0.05em !important;
          text-align: left !important;
        }
        .class-management-action-button {
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
    if (!window.confirm("Are you sure you want to delete this class level?")) return;
    try {
      const resp = await academicStructureService.deleteClassLevel(id);
      if (resp.isSuccess) {
        loadClasses();
      } else {
        alert(resp.message || "Failed to delete class level");
      }
    } catch (err: any) {
      alert("Error: " + err.message);
    }
  };

  return (
    <div>
      <ClassFormModal 
        isOpen={formModal.isOpen} 
        classLevel={formModal.classLevel}
        schemas={schemas}
        subjects={subjects}
        onConfirm={handleSave} 
        onCancel={() => setFormModal({ isOpen: false, apiError: '', classLevel: undefined })} 
        apiError={formModal.apiError}
      />
      
      <div style={styles.header} className="class-management-header">
        <h3 style={{ margin: 0 }}>Class Management</h3>
        <button 
          style={styles.createButton} 
          className="class-management-create-button"
          onClick={() => setFormModal({ isOpen: true, apiError: '', classLevel: undefined })}
        >
          Create Class
        </button>
      </div>
      
      {error && <ValidationMessage type="error" message={error} />}
      
      {loading ? <p>Loading classes...</p> : (
        <div style={styles.tableContainer} className="class-management-table-container">
          <table style={styles.table} className="class-management-table">
            <thead>
              <tr>
                <th style={styles.th}>Name</th>
                <th style={styles.th}>Section/Arm</th>
                <th style={styles.th}>Subjects</th>
                <th style={styles.th}>Sort Order</th>
                <th style={styles.th}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {classes.length === 0 ? (
                <tr><td colSpan={4} style={styles.td}>No classes found.</td></tr>
              ) : (
                classes.map(c => (
                  <tr key={c.id} style={styles.tr}>
                    <td style={styles.td} data-label="Name"><strong>{c.name}</strong></td>
                    <td style={styles.td} data-label="Section/Arm">{c.sectionOrArm || '-'}</td>
                    <td style={styles.td} data-label="Subjects">
                      <div style={{ display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
                        {c.subjectNames && c.subjectNames.length > 0 ? (
                          c.subjectNames.map((name, idx) => (
                            <span
                              key={idx}
                              style={{
                                display: 'inline-block',
                                padding: '2px 8px',
                                borderRadius: '12px',
                                fontSize: '11px',
                                fontWeight: 600,
                                backgroundColor: chipColors[idx % chipColors.length].bg,
                                color: chipColors[idx % chipColors.length].text,
                                whiteSpace: 'nowrap',
                              }}
                            >
                              {name}
                            </span>
                          ))
                        ) : (
                          <span style={{ fontSize: '12px', color: '#9ca3af', fontStyle: 'italic' }}>No subjects linked</span>
                        )}
                      </div>
                    </td>
                    <td style={styles.td} data-label="Order">{c.orderIndex}</td>
                    <td style={styles.td} data-label="Actions">
                      <button 
                        style={{ ...styles.actionButton, backgroundColor: '#3b82f6', marginRight: '8px' }} 
                        className="class-management-action-button"
                        onClick={() => setFormModal({ isOpen: true, apiError: '', classLevel: c })}
                      >
                        Edit
                      </button>
                      <button 
                        style={{ ...styles.actionButton, backgroundColor: '#ef4444' }} 
                        className="class-management-action-button"
                        onClick={() => handleDelete(c.id)}
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
