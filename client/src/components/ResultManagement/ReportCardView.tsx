import React, { useState, useEffect } from 'react';
import { AssessmentDetailsDto, UpdateAssessmentDetailsDto } from '../../models/academic';
import { resultService } from '../../services/resultService';
import { ValidationMessage } from '../Shared/ValidationComponents';
import { Sparkles } from 'lucide-react';

const gradeRemarkMap: Record<string, string> = {
  'A1': 'Excellent', 'B2': 'Very Good', 'B3': 'Good',
  'C4': 'Credit', 'C5': 'Credit', 'C6': 'Pass',
  'D7': 'Pass', 'E8': 'Poor', 'F9': 'Fail'
};

const getCalculatedGrade = (total: number) => {
  if (total >= 75) return 'A1';
  if (total >= 70) return 'B2';
  if (total >= 65) return 'B3';
  if (total >= 60) return 'C4';
  if (total >= 50) return 'C6';
  if (total >= 40) return 'E8';
  return 'F9';
};

interface ReportCardViewProps {
  assessmentId: string;
  onClose: () => void;
  onSaved: () => void;
  readOnly?: boolean;
}

const ReportCardView: React.FC<ReportCardViewProps> = ({ assessmentId, onClose, onSaved, readOnly = false }) => {
  const [report, setReport] = useState<AssessmentDetailsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // Form State
  const [formData, setFormData] = useState<UpdateAssessmentDetailsDto>({
    timesSchoolOpened: undefined,
    timesPresent: undefined,
    timesAbsent: undefined,
    regularity: '',
    punctuality: '',
    neatness: '',
    attitudeInSchool: '',
    socialActivities: '',
    indoorGames: '',
    fieldGames: '',
    trackGames: '',
    jumps: '',
    swims: '',
    classTeacherComment: '',
    headTeacherComment: ''
  });

  useEffect(() => {
    const fetchDetails = async () => {
      setLoading(true);
      try {
        const resp = await resultService.getReportCard(assessmentId);
        if (resp) {
          // Use type assertion if needed but we know it's AssessmentDetailsDto
          const data = resp as unknown as AssessmentDetailsDto;
          setReport(data);
          setFormData({
            timesSchoolOpened: data.timesSchoolOpened,
            timesPresent: data.timesPresent,
            timesAbsent: data.timesAbsent,
            regularity: data.regularity || '',
            punctuality: data.punctuality || '',
            neatness: data.neatness || '',
            attitudeInSchool: data.attitudeInSchool || '',
            socialActivities: data.socialActivities || '',
            indoorGames: data.indoorGames || '',
            fieldGames: data.fieldGames || '',
            trackGames: data.trackGames || '',
            jumps: data.jumps || '',
            swims: data.swims || '',
            classTeacherComment: data.classTeacherComment || '',
            headTeacherComment: data.headTeacherComment || ''
          });
        }
      } catch (err: any) {
        setError('Error: ' + err.message);
      } finally {
        setLoading(false);
      }
    };
    fetchDetails();
  }, [assessmentId]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    if (readOnly) return;
    const { name, value, type } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'number' ? (value ? parseInt(value) : undefined) : value
    }));
  };

  const handleSave = async () => {
    if (readOnly) return;
    setSaving(true);
    setError('');
    setSuccess('');

    try {
      await resultService.updateAssessmentDetails(assessmentId, formData);
      setSuccess('Report card details updated successfully.');
      setTimeout(() => {
        onSaved();
      }, 1500);
    } catch (err: any) {
      setError('Error saving: ' + err.message);
    } finally {
      setSaving(false);
    }
  };

  const handleAutoGenerateComments = () => {
    if (!report || readOnly) return;
    
    const avg = report.averageScore || 0;
    let academic = "showing steady progress";
    let traitGrade = 'C';
    
    if (avg >= 85) { academic = "demonstrating outstanding academic excellence"; traitGrade = 'A'; }
    else if (avg >= 70) { academic = "performing very well across most subjects"; traitGrade = 'B'; }
    else if (avg >= 50) { academic = "showing average performance but with room for improvement"; traitGrade = 'C'; }
    else { academic = "requiring more focus and dedication to improve academic performance"; traitGrade = 'D'; }

    const classComment = `${report.studentName} is ${academic}. Their positive attitude and good behavior are commendable. They also show great enthusiasm in physical activities.`;
    
    let headComment = "A satisfactory result.";
    if (avg >= 75) headComment = "An excellent result. Keep up the good work!";
    else if (avg >= 60) headComment = "A good performance. Aim higher next term.";
    else if (avg >= 50) headComment = "A fair performance. More effort is needed next term.";
    else headComment = "Poor performance. Parents should please monitor their studies at home.";

    setFormData(prev => ({
      ...prev,
      classTeacherComment: classComment,
      headTeacherComment: headComment,
      regularity: traitGrade,
      punctuality: traitGrade,
      neatness: traitGrade,
      attitudeInSchool: traitGrade,
      socialActivities: traitGrade,
      indoorGames: traitGrade,
      fieldGames: traitGrade,
      trackGames: traitGrade,
      jumps: traitGrade,
      swims: traitGrade
    }));
  };

  if (loading) return <div style={{ padding: '20px' }}>Loading Report Card...</div>;
  if (!report) return <div style={{ padding: '20px', color: 'red' }}>Report Card not found.</div>;

  return (
    <div style={{ backgroundColor: 'white', padding: '30px', borderRadius: '12px', width: '100%', maxWidth: '900px', maxHeight: '95vh', overflowY: 'auto', boxShadow: '0 25px 50px -12px rgba(0, 0, 0, 0.25)' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px', borderBottom: '1px solid #e5e7eb', paddingBottom: '16px' }}>
        <h2 style={{ margin: 0, fontSize: '24px', fontWeight: 700, color: '#111827' }}>
          {readOnly ? 'Student Report Card' : 'Edit Report Card'}: {report.studentName}
          <span style={{ fontSize: '14px', fontWeight: 400, color: '#6b7280', marginLeft: '12px' }}>
            Admn No: {report.admissionNumber || 'N/A'}
          </span>
        </h2>
        <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: '28px', cursor: 'pointer', color: '#9ca3af' }}>&times;</button>
      </div>

      {error && <ValidationMessage type="error" message={error} />}
      {success && <ValidationMessage type="success" message={success} />}

      {/* 1. Academic Table - CRITICAL for Report Card */}
      <div style={{ marginBottom: '32px' }}>
        <h3 style={{ fontSize: '16px', fontWeight: 600, color: '#374151', marginBottom: '12px', display: 'flex', alignItems: 'center' }}>
            <span style={{ width: '4px', height: '16px', backgroundColor: '#3b82f6', borderRadius: '2px', marginRight: '8px' }}></span>
            Academic Performance
        </h3>
        <div style={{ overflowX: 'auto', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                    <tr style={{ backgroundColor: '#f9fafb' }}>
                        <th style={tableThStyle}>Subject</th>
                        <th style={tableThStyle}>Test (40)</th>
                        <th style={tableThStyle}>Exam (60)</th>
                        <th style={tableThStyle}>Total (100)</th>
                        <th style={tableThStyle}>Grade</th>
                        <th style={tableThStyle}>Remark</th>
                    </tr>
                </thead>
                <tbody>
                    {report.subjectScores && report.subjectScores.length > 0 ? (
                        report.subjectScores.map((score, i) => {
                            const total = score.totalScore || ((score.testScore ?? 0) + (score.examScore ?? 0));
                            const calcGrade = score.grade && score.grade !== '-' ? score.grade : getCalculatedGrade(total);
                            const calcRemark = (score.subjectRemark && score.subjectRemark !== '-') ? score.subjectRemark : (gradeRemarkMap[calcGrade] || '-');
                            
                            return (
                                <tr key={i} style={{ borderBottom: i === report.subjectScores.length - 1 ? 'none' : '1px solid #f3f4f6' }}>
                                    <td style={tableTdStyle}><strong>{score.subjectName || 'Subject'}</strong></td>
                                    <td style={tableTdStyle}>{score.testScore ?? '-'}</td>
                                    <td style={tableTdStyle}>{score.examScore ?? '-'}</td>
                                    <td style={{ ...tableTdStyle, fontWeight: 700, color: total < 40 ? '#ef4444' : '#111827' }}>
                                        {total > 0 ? total : '-'}
                                    </td>
                                    <td style={tableTdStyle}>{calcGrade}</td>
                                    <td style={tableTdStyle}>{calcRemark}</td>
                                </tr>
                            );
                        })
                    ) : (
                        <tr><td colSpan={6} style={{ ...tableTdStyle, textAlign: 'center', color: '#9ca3af' }}>No scores available</td></tr>
                    )}
                </tbody>
            </table>
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', marginBottom: '32px' }}>
        {/* Academic Overview */}
        <div style={{ backgroundColor: '#eff6ff', padding: '20px', borderRadius: '12px', border: '1px solid #bfdbfe' }}>
          <h3 style={{ marginTop: 0, fontSize: '15px', fontWeight: 600, color: '#1e40af', marginBottom: '16px' }}>Term Summary</h3>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <SummaryItem label="Total Marks" value={`${report.totalMarksObtained ?? '-'} / ${report.totalMarksObtainable ?? '-'}`} />
            <SummaryItem label="Average" value={`${report.averageScore?.toFixed(2) ?? '-'}%`} />
            <SummaryItem label="Class Position" value={`${report.positionInClass ?? '-'} / ${report.numberInClass ?? '-'}`} />
            <SummaryItem label="Overall Grade" value={(report.overallGrade && report.overallGrade !== '-') ? report.overallGrade : (report.averageScore ? getCalculatedGrade(report.averageScore) : '-')} />
          </div>
        </div>

        {/* Attendance */}
        <div style={{ backgroundColor: '#fff', padding: '20px', borderRadius: '12px', border: '1px solid #e5e7eb' }}>
          <h3 style={{ marginTop: 0, fontSize: '15px', fontWeight: 600, color: '#374151', marginBottom: '16px' }}>Attendance</h3>
          <div style={{ display: 'grid', gap: '12px' }}>
            <div>
              <label style={labelStyle}>Times School Opened</label>
              <input type="number" name="timesSchoolOpened" style={inputStyle} value={formData.timesSchoolOpened || ''} onChange={handleInputChange} disabled={readOnly} />
            </div>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
              <div>
                <label style={labelStyle}>Times Present</label>
                <input type="number" name="timesPresent" style={inputStyle} value={formData.timesPresent || ''} onChange={handleInputChange} disabled={readOnly} />
              </div>
              <div>
                <label style={labelStyle}>Times Absent</label>
                <input type="number" name="timesAbsent" style={inputStyle} value={formData.timesAbsent || ''} onChange={handleInputChange} disabled={readOnly} />
              </div>
            </div>
          </div>
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', marginBottom: '32px' }}>
        {/* Affective Domain */}
        <div style={{ backgroundColor: '#fff', padding: '20px', borderRadius: '12px', border: '1px solid #e5e7eb' }}>
          <h3 style={{ marginTop: 0, fontSize: '15px', fontWeight: 600, color: '#374151', marginBottom: '16px' }}>Affective Domain (A-E)</h3>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <TraitInput label="Regularity" name="regularity" value={formData.regularity} onChange={handleInputChange} disabled={readOnly} />
            <TraitInput label="Punctuality" name="punctuality" value={formData.punctuality} onChange={handleInputChange} disabled={readOnly} />
            <TraitInput label="Neatness" name="neatness" value={formData.neatness} onChange={handleInputChange} disabled={readOnly} />
            <TraitInput label="Attitude" name="attitudeInSchool" value={formData.attitudeInSchool} onChange={handleInputChange} disabled={readOnly} />
            <TraitInput label="Social" name="socialActivities" value={formData.socialActivities} onChange={handleInputChange} disabled={readOnly} />
          </div>
        </div>

        {/* Psychomotor */}
        <div style={{ backgroundColor: '#fff', padding: '20px', borderRadius: '12px', border: '1px solid #e5e7eb' }}>
          <h3 style={{ marginTop: 0, fontSize: '15px', fontWeight: 600, color: '#374151', marginBottom: '16px' }}>Psychomotor Skills (A-E)</h3>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <TraitInput label="Indoor Games" name="indoorGames" value={formData.indoorGames} onChange={handleInputChange} disabled={readOnly} />
            <TraitInput label="Field Games" name="fieldGames" value={formData.fieldGames} onChange={handleInputChange} disabled={readOnly} />
            <TraitInput label="Track Games" name="trackGames" value={formData.trackGames} onChange={handleInputChange} disabled={readOnly} />
            <TraitInput label="Jumps" name="jumps" value={formData.jumps} onChange={handleInputChange} disabled={readOnly} />
            <TraitInput label="Swims" name="swims" value={formData.swims} onChange={handleInputChange} disabled={readOnly} />
          </div>
        </div>
      </div>

      {/* Remarks */}
      <div style={{ backgroundColor: '#fff', padding: '20px', borderRadius: '12px', border: '1px solid #e5e7eb', marginBottom: '32px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
          <h3 style={{ margin: 0, fontSize: '15px', fontWeight: 600, color: '#374151' }}>Comments & Remarks</h3>
          {!readOnly && (
            <button 
              onClick={handleAutoGenerateComments} 
              style={{ display: 'flex', alignItems: 'center', gap: '6px', padding: '6px 12px', backgroundColor: '#f0fdf4', color: '#166534', border: '1px solid #bbf7d0', borderRadius: '6px', cursor: 'pointer', fontSize: '13px', fontWeight: 600 }}
              title="Generate comments based on student's performance"
            >
              <Sparkles size={14} /> Auto-Generate (AI)
            </button>
          )}
        </div>
        <div style={{ display: 'grid', gap: '20px' }}>
          <div>
            <label style={labelStyle}>Class Teacher's Comment</label>
            <textarea name="classTeacherComment" style={textareaStyle} value={formData.classTeacherComment} onChange={handleInputChange} disabled={readOnly} placeholder="Enter class teacher's assessment..."></textarea>
          </div>
          <div>
            <label style={labelStyle}>Head Teacher's Comment</label>
            <textarea name="headTeacherComment" style={textareaStyle} value={formData.headTeacherComment} onChange={handleInputChange} disabled={readOnly} placeholder="Enter head teacher's final remark..."></textarea>
          </div>
        </div>
      </div>

      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', borderTop: '1px solid #e5e7eb', paddingTop: '20px' }}>
        <button onClick={onClose} style={{ padding: '10px 20px', backgroundColor: '#f3f4f6', color: '#374151', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 600 }}>
            {readOnly ? 'Close' : 'Cancel'}
        </button>
        {!readOnly && (
            <button onClick={handleSave} disabled={saving} style={{ padding: '10px 24px', backgroundColor: '#2563eb', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 600 }}>
              {saving ? 'Saving...' : 'Save Report Details'}
            </button>
        )}
      </div>
    </div>
  );
};

const TraitInput = ({ label, name, value, onChange, disabled }: { label: string, name: string, value: string | undefined, onChange: any, disabled: boolean }) => (
  <div>
    <label style={{ display: 'block', fontSize: '11px', fontWeight: 600, marginBottom: '4px', color: '#6b7280', textTransform: 'uppercase' }}>{label}</label>
    <input type="text" name={name} style={{...inputStyle, textAlign: 'center'}} value={value || ''} onChange={onChange} maxLength={5} placeholder="-" disabled={disabled} />
  </div>
);

const SummaryItem: React.FC<{ label: string, value: string | number }> = ({ label, value }) => (
  <div style={{ display: 'flex', flexDirection: 'column' }}>
    <span style={{ fontSize: '13px', color: '#3b82f6', marginBottom: '4px' }}>{label}</span>
    <span style={{ fontSize: '18px', fontWeight: 700, color: '#1e3a8a' }}>{value}</span>
  </div>
);

const tableThStyle: React.CSSProperties = {
    textAlign: 'left',
    padding: '12px 16px',
    fontSize: '13px',
    fontWeight: 600,
    color: '#4b5563',
    borderBottom: '1px solid #e5e7eb'
};

const tableTdStyle: React.CSSProperties = {
    padding: '12px 16px',
    fontSize: '14px',
    color: '#374151',
};

const labelStyle: React.CSSProperties = {
  display: 'block',
  fontSize: '13px',
  fontWeight: 600,
  marginBottom: '6px',
  color: '#374151'
};

const inputStyle: React.CSSProperties = {
  width: '100%',
  padding: '10px 12px',
  borderRadius: '8px',
  border: '1px solid #d1d5db',
  fontSize: '14px',
  boxSizing: 'border-box',
  backgroundColor: '#fff',
  transition: 'border-color 0.2s',
  outline: 'none'
};

const textareaStyle: React.CSSProperties = {
  ...inputStyle,
  minHeight: '100px',
  resize: 'vertical'
};

export default ReportCardView;
