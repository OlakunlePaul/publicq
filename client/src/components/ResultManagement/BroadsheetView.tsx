import React, { useState, useEffect } from 'react';

import { SubjectDto, BulkScoreEntryDto } from '../../models/academic';
import { resultService, StudentAssessment } from '../../services/resultService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface BroadsheetViewProps {
    sessionId: string;
    termId: string;
    classLevelId: string;
    subjects: SubjectDto[];
    onClose: () => void;
}

const BroadsheetView: React.FC<BroadsheetViewProps> = ({ sessionId, termId, classLevelId, subjects, onClose }) => {
    const [assessments, setAssessments] = useState<StudentAssessment[]>([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    
    // localScores[examTakerId][subjectId] = { test: '', exam: '' }
    const [localScores, setLocalScores] = useState<Record<string, Record<string, { test: string, exam: string }>>>({});

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const data = await resultService.getClassResults(sessionId, termId, classLevelId);
                setAssessments(data);
                
                // Initialize local scores map
                const scoresMap: Record<string, Record<string, { test: string, exam: string }>> = {};
                data.forEach(stu => {
                    scoresMap[stu.examTakerId] = {};
                    subjects.forEach(sub => {
                        const existing = stu.subjectScores.find(ss => ss.subjectName === sub.name);
                        scoresMap[stu.examTakerId][sub.id] = {
                            test: existing?.testScore?.toString() || '',
                            exam: existing?.examScore?.toString() || ''
                        };
                    });
                });
                setLocalScores(scoresMap);
            } catch (err: any) {
                setError('Failed to load broadsheet data: ' + err.message);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, [sessionId, termId, classLevelId, subjects]);

    const handleScoreChange = (examTakerId: string, subjectId: string, field: 'test' | 'exam', value: string) => {
        setLocalScores(prev => ({
            ...prev,
            [examTakerId]: {
                ...prev[examTakerId],
                [subjectId]: {
                    ...prev[examTakerId][subjectId],
                    [field]: value
                }
            }
        }));
    };

    const handleSaveAll = async () => {
        setSaving(true);
        setError('');
        setSuccess('');

        const allScores: any[] = [];
        Object.entries(localScores).forEach(([examTakerId, subjectMap]) => {
            Object.entries(subjectMap).forEach(([subjectId, score]) => {
                // Only send if at least one score is entered
                if (score.test !== '' || score.exam !== '') {
                    allScores.push({
                        examTakerId,
                        subjectId,
                        testScore: score.test !== '' ? parseFloat(score.test) : 0,
                        examScore: score.exam !== '' ? parseFloat(score.exam) : 0
                    });
                }
            });
        });

        if (allScores.length === 0) {
            setSaving(false);
            return;
        }

        const payload: BulkScoreEntryDto = {
            sessionId,
            termId,
            classLevelId,
            scores: allScores
        };

        try {
            const resp = await resultService.saveBulkScores(payload);
            if (resp.isSuccess) {
                setSuccess('Broadsheet scores saved successfully.');
            } else {
                setError(resp.message || 'Failed to save scores.');
            }
        } catch (err: any) {
            setError('Error saving scores: ' + err.message);
        } finally {
            setSaving(false);
        }
    };

    if (loading) return <div>Loading Broadsheet...</div>;

    return (
        <div style={containerStyle}>
            <div style={headerStyle}>
                <h3 style={{ margin: 0 }}>Class Broadsheet (Manual Entry)</h3>
                <div style={{ display: 'flex', gap: '12px' }}>
                    <button onClick={onClose} style={cancelButtonStyle}>Close</button>
                    <button onClick={handleSaveAll} style={saveButtonStyle} disabled={saving}>
                        {saving ? 'Saving...' : 'Save All Changes'}
                    </button>
                </div>
            </div>

            {error && <ValidationMessage type="error" message={error} />}
            {success && <ValidationMessage type="success" message={success} />}

            <div style={scrollContainerStyle}>
                <table style={tableStyle}>
                    <thead>
                        <tr>
                            <th style={stickyThStyle} rowSpan={2}>Student Name</th>
                            {subjects.map(sub => (
                                <th key={sub.id} style={subjectThStyle} colSpan={2}>{sub.name}</th>
                            ))}
                            <th style={thStyle} rowSpan={2}>Total</th>
                            <th style={thStyle} rowSpan={2}>Avg</th>
                        </tr>
                        <tr>
                            {subjects.map(sub => (
                                <React.Fragment key={sub.id + '-sub'}>
                                    <th style={subThStyle}>T</th>
                                    <th style={subThStyle}>E</th>
                                </React.Fragment>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {assessments.map(stu => (
                            <tr key={stu.id} style={trStyle}>
                                <td style={stickyTdStyle}><strong>{stu.studentName}</strong></td>
                                {subjects.map(sub => (
                                    <React.Fragment key={stu.id + sub.id}>
                                        <td style={tdStyle}>
                                            <input 
                                                type="number" 
                                                style={inputStyle} 
                                                value={localScores[stu.examTakerId]?.[sub.id]?.test || ''}
                                                onChange={(e) => handleScoreChange(stu.examTakerId, sub.id, 'test', e.target.value)}
                                            />
                                        </td>
                                        <td style={tdStyle}>
                                            <input 
                                                type="number" 
                                                style={inputStyle} 
                                                value={localScores[stu.examTakerId]?.[sub.id]?.exam || ''}
                                                onChange={(e) => handleScoreChange(stu.examTakerId, sub.id, 'exam', e.target.value)}
                                            />
                                        </td>
                                    </React.Fragment>
                                ))}
                                <td style={tdStyle}>{stu.totalMarksObtained?.toFixed(1) || '0'}</td>
                                <td style={tdStyle}>{stu.averageScore?.toFixed(1) || '0'}%</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

const containerStyle: React.CSSProperties = {
    backgroundColor: 'white',
    padding: '24px',
    borderRadius: '12px',
    width: '95vw',
    height: '90vh',
    display: 'flex',
    flexDirection: 'column',
    overflow: 'hidden'
};

const headerStyle: React.CSSProperties = {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '20px'
};

const scrollContainerStyle: React.CSSProperties = {
    flex: 1,
    overflow: 'auto',
    border: '1px solid #e5e7eb',
    borderRadius: '8px'
};

const tableStyle: React.CSSProperties = {
    width: 'max-content',
    minWidth: '100%',
    borderCollapse: 'separate',
    borderSpacing: 0
};

const thStyle: React.CSSProperties = {
    backgroundColor: '#f3f4f6',
    borderBottom: '2px solid #e5e7eb',
    borderRight: '1px solid #e5e7eb',
    padding: '8px 12px',
    fontSize: '13px',
    fontWeight: 600,
    textAlign: 'center',
    position: 'sticky',
    top: 0,
    zIndex: 10
};

const stickyThStyle: React.CSSProperties = {
    ...thStyle,
    position: 'sticky',
    left: 0,
    zIndex: 20,
    backgroundColor: '#f3f4f6',
    width: '200px',
    textAlign: 'left'
};

const subjectThStyle: React.CSSProperties = {
    ...thStyle,
    backgroundColor: '#eef2ff',
    color: '#4338ca'
};

const subThStyle: React.CSSProperties = {
    ...thStyle,
    fontSize: '11px',
    padding: '4px'
};

const trStyle: React.CSSProperties = {
    borderBottom: '1px solid #e5e7eb'
};

const tdStyle: React.CSSProperties = {
    padding: '8px',
    fontSize: '13px',
    borderBottom: '1px solid #e5e7eb',
    borderRight: '1px solid #e5e7eb',
    textAlign: 'center'
};

const stickyTdStyle: React.CSSProperties = {
    ...tdStyle,
    position: 'sticky',
    left: 0,
    zIndex: 5,
    backgroundColor: 'white',
    textAlign: 'left',
    width: '200px',
    borderRight: '2px solid #e5e7eb'
};

const inputStyle: React.CSSProperties = {
    width: '45px',
    padding: '4px',
    fontSize: '12px',
    borderRadius: '4px',
    border: '1px solid #d1d5db',
    textAlign: 'center'
};

const saveButtonStyle: React.CSSProperties = {
    padding: '8px 16px',
    backgroundColor: '#10b981',
    color: 'white',
    border: 'none',
    borderRadius: '6px',
    fontWeight: 600,
    cursor: 'pointer'
};

const cancelButtonStyle: React.CSSProperties = {
    padding: '8px 16px',
    backgroundColor: '#e5e7eb',
    color: '#374151',
    border: 'none',
    borderRadius: '6px',
    fontWeight: 600,
    cursor: 'pointer'
};

export default BroadsheetView;
