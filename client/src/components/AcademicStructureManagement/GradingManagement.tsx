import React, { useState, useEffect } from 'react';
import commonStyles from '../Admin/AdminCommon.module.css';
import { academicStructureService } from '../../services/academicStructureService';
import { GradingSchemaDto, GradeRangeDto } from '../../models/academic';
import { ValidationMessage } from '../Shared/ValidationComponents';

const GradingManagement: React.FC = () => {
    const [schemas, setSchemas] = useState<GradingSchemaDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [editingSchema, setEditingSchema] = useState<GradingSchemaDto | null>(null);

    // Form state
    const [formData, setFormData] = useState({
        name: '',
        isActive: true,
        gradeRanges: [] as GradeRangeDto[]
    });

    useEffect(() => {
        loadSchemas();
    }, []);

    const loadSchemas = async () => {
        try {
            const resp = await academicStructureService.getGradingSchemas();
            if (resp.isSuccess) setSchemas(resp.data || []);
        } catch (err: any) {
            setError('Failed to load grading schemas');
        } finally {
            setLoading(false);
        }
    };

    const handleAddGradeRange = () => {
        setFormData(prev => ({
            ...prev,
            gradeRanges: [...prev.gradeRanges, { symbol: '', minScore: 0, maxScore: 0, remark: '' }]
        }));
    };

    const handleRemoveGradeRange = (index: number) => {
        setFormData(prev => ({
            ...prev,
            gradeRanges: prev.gradeRanges.filter((_, i) => i !== index)
        }));
    };

    const handleRangeChange = (index: number, field: keyof GradeRangeDto, value: any) => {
        const updated = [...formData.gradeRanges];
        updated[index] = { ...updated[index], [field]: value };
        setFormData(prev => ({ ...prev, gradeRanges: updated }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        
        try {
            let resp;
            if (editingSchema) {
                resp = await academicStructureService.updateGradingSchema(editingSchema.id, formData);
            } else {
                resp = await academicStructureService.createGradingSchema(formData);
            }

            if (resp.isSuccess) {
                setSuccess('Grading schema saved successfully');
                setShowModal(false);
                loadSchemas();
            } else {
                setError(resp.message || 'Failed to save schema');
            }
        } catch (err: any) {
            setError('Error saving schema');
        } finally {
            setLoading(false);
        }
    };

    const openEditModal = (schema: GradingSchemaDto) => {
        setEditingSchema(schema);
        setFormData({
            name: schema.name,
            isActive: schema.isActive,
            gradeRanges: [...schema.gradeRanges]
        });
        setShowModal(true);
    };

    const openCreateModal = () => {
        setEditingSchema(null);
        setFormData({
            name: '',
            isActive: true,
            gradeRanges: [
                { symbol: 'A', minScore: 70, maxScore: 100, remark: 'Excellent' },
                { symbol: 'B', minScore: 60, maxScore: 69, remark: 'Very Good' },
                { symbol: 'C', minScore: 50, maxScore: 59, remark: 'Good' },
                { symbol: 'F', minScore: 0, maxScore: 49, remark: 'Fail' }
            ]
        });
        setShowModal(true);
    };

    if (loading && schemas.length === 0) return <div>Loading...</div>;

    return (
        <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h3 style={{ margin: 0 }}>Grading Schemas</h3>
                <button 
                    onClick={openCreateModal}
                    className={commonStyles.primaryButton}
                >
                    Create New Schema
                </button>
            </div>

            {error && <ValidationMessage type="error" message={error} />}
            {success && <ValidationMessage type="success" message={success} />}

            <div className={commonStyles.card} style={{ padding: 0 }}>
                <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                    <thead>
                        <tr style={{ textAlign: 'left', backgroundColor: '#f9fafb', borderBottom: '1px solid #e5e7eb' }}>
                            <th style={{ padding: '12px 16px' }}>Name</th>
                            <th style={{ padding: '12px 16px' }}>Status</th>
                            <th style={{ padding: '12px 16px' }}>Grades</th>
                            <th style={{ padding: '12px 16px' }}>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {schemas.map(s => (
                            <tr key={s.id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                                <td style={{ padding: '12px 16px' }}><strong>{s.name}</strong></td>
                                <td style={{ padding: '12px 16px' }}>
                                    <span style={{ 
                                        padding: '4px 8px', borderRadius: '4px', fontSize: '12px',
                                        backgroundColor: s.isActive ? '#dcfce7' : '#fee2e2',
                                        color: s.isActive ? '#166534' : '#991b1b'
                                    }}>
                                        {s.isActive ? 'Active' : 'Inactive'}
                                    </span>
                                </td>
                                <td style={{ padding: '12px 16px' }}>
                                    {s.gradeRanges.map(r => r.symbol).join(', ')}
                                </td>
                                <td style={{ padding: '12px 16px' }}>
                                    <button 
                                        onClick={() => openEditModal(s)}
                                        style={{ background: 'none', border: 'none', color: '#3b82f6', cursor: 'pointer', fontWeight: 600 }}
                                    >
                                        Edit
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {showModal && (
                <div style={modalOverlayStyle}>
                    <div className={commonStyles.card} style={{ width: '600px', maxHeight: '90vh', overflowY: 'auto' }}>
                        <h3>{editingSchema ? 'Edit' : 'Create'} Grading Schema</h3>
                        <form onSubmit={handleSubmit}>
                            <div className={commonStyles.formGroup}>
                                <label className={commonStyles.formLabel}>Schema Name</label>
                                <input 
                                    type="text" 
                                    className={commonStyles.formInput}
                                    value={formData.name}
                                    onChange={e => setFormData({ ...formData, name: e.target.value })}
                                    required
                                    placeholder="e.g., Default Primary"
                                />
                            </div>

                            <div className={commonStyles.formGroup}>
                                <h4 style={{ marginBottom: '10px' }}>Grade Ranges</h4>
                                <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                                    {formData.gradeRanges.map((range, index) => (
                                        <div key={index} style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
                                            <input 
                                                style={{ width: '60px', padding: '8px', border: '1px solid #d1d5db', borderRadius: '4px' }}
                                                placeholder="Symbol"
                                                value={range.symbol}
                                                onChange={e => handleRangeChange(index, 'symbol', e.target.value)}
                                                required
                                            />
                                            <input 
                                                type="number"
                                                style={{ width: '70px', padding: '8px', border: '1px solid #d1d5db', borderRadius: '4px' }}
                                                placeholder="Min"
                                                value={range.minScore}
                                                onChange={e => handleRangeChange(index, 'minScore', parseInt(e.target.value))}
                                                required
                                            />
                                            <span>to</span>
                                            <input 
                                                type="number"
                                                style={{ width: '70px', padding: '8px', border: '1px solid #d1d5db', borderRadius: '4px' }}
                                                placeholder="Max"
                                                value={range.maxScore}
                                                onChange={e => handleRangeChange(index, 'maxScore', parseInt(e.target.value))}
                                                required
                                            />
                                            <input 
                                                style={{ flex: 1, padding: '8px', border: '1px solid #d1d5db', borderRadius: '4px' }}
                                                placeholder="Remark"
                                                value={range.remark}
                                                onChange={e => handleRangeChange(index, 'remark', e.target.value)}
                                            />
                                            <button 
                                                type="button"
                                                onClick={() => handleRemoveGradeRange(index)}
                                                style={{ padding: '8px', color: '#dc2626', border: 'none', background: 'none', cursor: 'pointer' }}
                                            >
                                                ✕
                                            </button>
                                        </div>
                                    ))}
                                </div>
                                <button 
                                    type="button"
                                    onClick={handleAddGradeRange}
                                    style={{ marginTop: '12px', padding: '6px 12px', backgroundColor: '#f3f4f6', border: '1px solid #d1d5db', borderRadius: '4px', cursor: 'pointer' }}
                                >
                                    + Add Grade
                                </button>
                            </div>

                            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '24px' }}>
                                <button 
                                    type="button"
                                    onClick={() => setShowModal(false)}
                                    className={commonStyles.secondaryButton}
                                >
                                    Cancel
                                </button>
                                <button 
                                    type="submit"
                                    className={commonStyles.primaryButton}
                                    disabled={loading}
                                >
                                    {loading ? 'Saving...' : 'Save Schema'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};

const modalOverlayStyle: React.CSSProperties = {
    position: 'fixed',
    top: 0, left: 0, right: 0, bottom: 0,
    backgroundColor: 'rgba(0,0,0,0.5)',
    display: 'flex', justifyContent: 'center', alignItems: 'center',
    zIndex: 1000,
    padding: '20px'
};

export default GradingManagement;
