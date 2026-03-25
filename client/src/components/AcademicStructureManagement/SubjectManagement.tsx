import React, { useEffect, useState, useCallback } from 'react';
import { SubjectDto, SubjectCreateDto, ClassLevelDto } from '../../models/academic';
import { academicStructureService } from '../../services/academicStructureService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface SubjectFormModalProps {
  isOpen: boolean;
  subject?: SubjectDto; // Pass subject for editing
  classLevels: ClassLevelDto[];
  onConfirm: (subject: SubjectCreateDto) => void;
  onCancel: () => void;
  apiError?: string;
}

const SubjectFormModal = ({ isOpen, subject, classLevels, onConfirm, onCancel, apiError }: SubjectFormModalProps) => {
  const [formData, setFormData] = useState<SubjectCreateDto>({
    name: '',
    code: '',
    displayOrder: 0,
    classLevelIds: []
  });
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      if (subject) {
        setFormData({ 
          name: subject.name, 
          code: subject.code || '', 
          displayOrder: subject.displayOrder,
          classLevelIds: subject.classLevelIds || []
        });
      } else {
        setFormData({ name: '', code: '', displayOrder: 0, classLevelIds: [] });
      }
      setError('');
    }
  }, [isOpen, subject]);

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

  const toggleClass = (classId: string) => {
    const currentIds = formData.classLevelIds || [];
    if (currentIds.includes(classId)) {
      setFormData({ ...formData, classLevelIds: currentIds.filter(id => id !== classId) });
    } else {
      setFormData({ ...formData, classLevelIds: [...currentIds, classId] });
    }
  };

  const toggleAllClasses = () => {
    if ((formData.classLevelIds?.length || 0) === classLevels.length) {
      setFormData({ ...formData, classLevelIds: [] });
    } else {
      setFormData({ ...formData, classLevelIds: classLevels.map(cl => cl.id) });
    }
  };

  if (!isOpen) return null;

  return (
    <div style={styles.modalOverlay}>
      <div style={{ ...styles.modal, width: '600px', maxWidth: '95%' }}>
        <h3 style={styles.modalTitle}>{subject ? 'Edit Subject' : 'Create New Subject'}</h3>
        {error && <ValidationMessage type="error" message={error} />}
        
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px' }}>
          <div>
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
              <label style={styles.formLabel}>Subject Code:</label>
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
              <p style={{ fontSize: '11px', color: '#6b7280', marginTop: '4px' }}>Lower numbers appear first on report cards.</p>
            </div>
          </div>

          <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' }}>
              <label style={styles.formLabel}>Link to Classes:</label>
              {classLevels.length > 0 && (
                <button 
                  onClick={toggleAllClasses}
                  style={{ fontSize: '12px', color: '#3b82f6', background: 'none', border: 'none', cursor: 'pointer', padding: 0 }}
                >
                  {(formData.classLevelIds?.length || 0) === classLevels.length ? 'Deselect All' : 'Select All'}
                </button>
              )}
            </div>
            <div style={{ 
              height: '240px', 
              overflowY: 'auto', 
              border: '1px solid #d1d5db', 
              borderRadius: '8px',
              padding: '12px',
              backgroundColor: '#f9fafb'
            }}>
              {classLevels.length === 0 ? (
                <p style={{ fontSize: '12px', color: '#6b7280' }}>No classes available. Create classes first.</p>
              ) : (
                <div style={{ display: 'grid', gridTemplateColumns: '1fr', gap: '4px' }}>
                  {classLevels.map(cl => (
                    <div 
                      key={cl.id} 
                      onClick={() => toggleClass(cl.id)}
                      style={{ 
                        display: 'flex', 
                        alignItems: 'center', 
                        padding: '6px 8px', 
                        borderRadius: '6px',
                        cursor: 'pointer',
                        backgroundColor: formData.classLevelIds?.includes(cl.id) ? '#eff6ff' : 'transparent',
                        transition: 'background-color 0.2s'
                      }}
                    >
                      <input 
                        type="checkbox" 
                        id={`class-${cl.id}`}
                        checked={formData.classLevelIds?.includes(cl.id)}
                        onChange={(e) => { e.stopPropagation(); toggleClass(cl.id); }}
                        style={{ marginRight: '10px', cursor: 'pointer' }}
                      />
                      <label 
                        htmlFor={`class-${cl.id}`} 
                        onClick={(e) => e.preventDefault()}
                        style={{ fontSize: '13px', cursor: 'pointer', color: '#374151', flex: 1 }}
                      >
                        {cl.name} {cl.sectionOrArm ? `(${cl.sectionOrArm})` : ''}
                      </label>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>

        <div style={styles.modalActions}>
          <button onClick={onCancel} style={styles.modalCancelButton}>Cancel</button>
          <button onClick={handleConfirm} style={styles.modalConfirmButton}>{subject ? 'Update Subject' : 'Create Subject'}</button>
        </div>
      </div>
    </div>
  );
};

// Color palette for class chips — 8 visually distinct pairs
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

const SubjectManagement = () => {
  const [subjects, setSubjects] = useState<SubjectDto[]>([]);
  const [classLevels, setClassLevels] = useState<ClassLevelDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [formModal, setFormModal] = useState<{ isOpen: boolean; apiError: string; subject?: SubjectDto }>({ isOpen: false, apiError: '' });
  const [classFilter, setClassFilter] = useState<string>('all');

  const loadSubjects = useCallback(async () => {
    setLoading(true);
    try {
      const [subjectResp, classResp] = await Promise.all([
        academicStructureService.getSubjects(),
        academicStructureService.getClassLevels()
      ]);
      
      if (subjectResp.isSuccess) {
        setSubjects((subjectResp.data || []).sort((a, b) => a.displayOrder - b.displayOrder));
      }
      if (classResp.isSuccess) {
        setClassLevels((classResp.data || []).sort((a, b) => a.orderIndex - b.orderIndex));
      }
    } catch (err: any) {
      setError('Failed to load subjects or classes: ' + err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadSubjects();
  }, [loadSubjects]);

  const handleSave = async (data: SubjectCreateDto) => {
    // Client-side validation: Check for duplicate name if creating new
    if (!formModal.subject) {
      const duplicate = subjects.find(s => s.name.trim().toLowerCase() === data.name.trim().toLowerCase());
      if (duplicate) {
        setFormModal({ ...formModal, apiError: `Subject '${data.name}' already exists. Please find and edit the existing subject to link it to more classes.` });
        return;
      }
    }

    try {
      let resp;
      if (formModal.subject) {
        resp = await academicStructureService.updateSubject(formModal.subject.id, data);
      } else {
        resp = await academicStructureService.createSubject(data);
      }

      if (resp.isSuccess) {
        setFormModal({ isOpen: false, apiError: '', subject: undefined });
        loadSubjects();
      } else {
        setFormModal({ ...formModal, apiError: resp.message || 'Failed to save subject' });
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
        .subject-management-header {
          flex-direction: column !important;
          align-items: stretch !important;
          gap: 16px !important;
          margin-bottom: 24px !important;
        }
        .subject-management-header h3 {
          text-align: center !important;
        }
        .subject-management-create-button {
          width: 100% !important;
          max-width: 300px !important;
          margin: 0 auto !important;
          height: 48px !important;
        }
        .subject-management-table-container {
          border: none !important;
          box-shadow: none !important;
          background: transparent !important;
          overflow: visible !important;
        }
        .subject-management-table, 
        .subject-management-table thead, 
        .subject-management-table tbody, 
        .subject-management-table tr, 
        .subject-management-table td {
          display: block !important;
          width: 100% !important;
        }
        .subject-management-table thead {
          display: none !important;
        }
        .subject-management-table tr {
          background-color: white !important;
          border-radius: 16px !important;
          margin-bottom: 20px !important;
          padding: 16px !important;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05), 0 1px 2px rgba(0, 0, 0, 0.1) !important;
          border: 1px solid #f3f4f6 !important;
        }
        .subject-management-table td {
          border-bottom: 1px solid #f9fafb !important;
          padding: 12px 0 !important;
          display: flex !important;
          justify-content: space-between !important;
          align-items: center !important;
          text-align: right !important;
          min-height: 44px !important;
        }
        .subject-management-table td:last-child {
          border-bottom: none !important;
          margin-top: 12px !important;
          flex-direction: column !important;
          align-items: stretch !important;
          text-align: left !important;
        }
        .subject-management-table td::before {
          content: attr(data-label) !important;
          font-weight: 700 !important;
          color: #6b7280 !important;
          text-transform: uppercase !important;
          font-size: 11px !important;
          letter-spacing: 0.05em !important;
          text-align: left !important;
        }
        .subject-management-action-button {
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
    if (!window.confirm("Are you sure you want to delete this subject?")) return;
    try {
      const resp = await academicStructureService.deleteSubject(id);
      if (resp.isSuccess) {
        loadSubjects();
      } else {
        alert(resp.message || "Failed to delete subject");
      }
    } catch (err: any) {
      alert("Error: " + err.message);
    }
  };

  return (
    <div>
      <SubjectFormModal 
        isOpen={formModal.isOpen} 
        subject={formModal.subject}
        classLevels={classLevels}
        onConfirm={handleSave} 
        onCancel={() => setFormModal({ isOpen: false, apiError: '', subject: undefined })} 
        apiError={formModal.apiError}
      />
      
      <div style={styles.header} className="subject-management-header">
        <h3 style={{ margin: 0 }}>Subject Management</h3>
        <button 
          style={styles.createButton} 
          className="subject-management-create-button"
          onClick={() => setFormModal({ isOpen: true, apiError: '', subject: undefined })}
        >
          Create Subject
        </button>
      </div>

      {/* Class Filter */}
      <div style={{ marginBottom: '16px', display: 'flex', alignItems: 'center', gap: '12px' }}>
        <label style={{ fontSize: '14px', fontWeight: 600, color: '#374151' }}>Filter by Class:</label>
        <select
          value={classFilter}
          onChange={(e) => setClassFilter(e.target.value)}
          style={{
            padding: '8px 12px',
            borderRadius: '8px',
            border: '1px solid #d1d5db',
            fontSize: '14px',
            color: '#374151',
            backgroundColor: '#fff',
            cursor: 'pointer',
            minWidth: '180px',
          }}
        >
          <option value="all">All Classes</option>
          {classLevels.map(cl => (
            <option key={cl.id} value={cl.id}>{cl.name}{cl.sectionOrArm ? ` (${cl.sectionOrArm})` : ''}</option>
          ))}
          <option value="none">Unlinked (No Class)</option>
        </select>
        {classFilter !== 'all' && (
          <button
            onClick={() => setClassFilter('all')}
            style={{ padding: '6px 12px', borderRadius: '6px', border: '1px solid #d1d5db', backgroundColor: '#f9fafb', cursor: 'pointer', fontSize: '13px', color: '#6b7280' }}
          >
            ✕ Clear
          </button>
        )}
      </div>
      
      {error && <ValidationMessage type="error" message={error} />}
      
      {loading ? <p>Loading subjects...</p> : (
        <div style={styles.tableContainer} className="subject-management-table-container">
          <table style={styles.table} className="subject-management-table">
            <thead>
              <tr>
                <th style={styles.th}>Name</th>
                <th style={styles.th}>Code</th>
                <th style={styles.th}>Classes</th>
                <th style={styles.th}>Display Order</th>
                <th style={styles.th}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {subjects.length === 0 ? (
                <tr><td colSpan={4} style={styles.td}>No subjects found.</td></tr>
              ) : (
               subjects
                .filter(s => {
                  if (classFilter === 'all') return true;
                  if (classFilter === 'none') return !s.classLevelIds || s.classLevelIds.length === 0;
                  return s.classLevelIds?.includes(classFilter);
                })
                .map(s => (
                  <tr key={s.id} style={styles.tr}>
                    <td style={styles.td} data-label="Name"><strong>{s.name}</strong></td>
                    <td style={styles.td} data-label="Code">{s.code || '-'}</td>
                    <td style={styles.td} data-label="Classes">
                      <div style={{ display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
                        {s.classLevelNames && s.classLevelNames.length > 0 ? (
                          s.classLevelNames.map((name, idx) => (
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
                          <span style={{ fontSize: '12px', color: '#9ca3af', fontStyle: 'italic' }}>No class linked</span>
                        )}
                      </div>
                    </td>
                    <td style={styles.td} data-label="Order">{s.displayOrder}</td>
                    <td style={styles.td} data-label="Actions">
                      <button 
                        style={{ ...styles.actionButton, backgroundColor: '#3b82f6', marginRight: '8px' }} 
                        className="subject-management-action-button"
                        onClick={() => setFormModal({ isOpen: true, apiError: '', subject: s })}
                      >
                        Edit
                      </button>
                      <button 
                        style={{ ...styles.actionButton, backgroundColor: '#ef4444' }} 
                        className="subject-management-action-button"
                        onClick={() => handleDelete(s.id)}
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

export default SubjectManagement;
