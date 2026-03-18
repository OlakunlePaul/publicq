import React, { useState, useEffect } from 'react';
import commonStyles from '../Admin/AdminCommon.module.css';
import { AssessmentDetailsDto, UpdateAssessmentDetailsDto } from '../../models/academic';
import { resultService } from '../../services/resultService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface ReportCardViewProps {
  assessmentId: string;
  onClose: () => void;
  onSaved: () => void;
}

const ReportCardView: React.FC<ReportCardViewProps> = ({ assessmentId, onClose, onSaved }) => {
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
        const resp = await resultService.getAssessmentDetails(assessmentId);
        if (resp.isSuccess && resp.data) {
          setReport(resp.data);
          setFormData({
            timesSchoolOpened: resp.data.timesSchoolOpened,
            timesPresent: resp.data.timesPresent,
            timesAbsent: resp.data.timesAbsent,
            regularity: resp.data.regularity || '',
            punctuality: resp.data.punctuality || '',
            neatness: resp.data.neatness || '',
            attitudeInSchool: resp.data.attitudeInSchool || '',
            socialActivities: resp.data.socialActivities || '',
            indoorGames: resp.data.indoorGames || '',
            fieldGames: resp.data.fieldGames || '',
            trackGames: resp.data.trackGames || '',
            jumps: resp.data.jumps || '',
            swims: resp.data.swims || '',
            classTeacherComment: resp.data.classTeacherComment || '',
            headTeacherComment: resp.data.headTeacherComment || ''
          });
        } else {
          setError(resp.message || 'Failed to load report card details.');
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
    const { name, value, type } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'number' ? (value ? parseInt(value) : undefined) : value
    }));
  };

  const handleSave = async () => {
    setSaving(true);
    setError('');
    setSuccess('');

    try {
      const resp = await resultService.updateAssessmentDetails(assessmentId, formData);
      if (resp.isSuccess) {
        setSuccess('Report card details updated successfully.');
        setTimeout(() => {
          onSaved();
        }, 1500);
      } else {
        setError(resp.message || 'Failed to save details.');
      }
    } catch (err: any) {
      setError('Error saving: ' + err.message);
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <div style={{ padding: '20px' }}>Loading Report Card...</div>;
  if (!report) return <div style={{ padding: '20px', color: 'red' }}>Report Card not found.</div>;

  return (
    <div style={{ backgroundColor: 'white', padding: '30px', borderRadius: '12px', width: '100%', maxWidth: '900px', maxHeight: '90vh', overflowY: 'auto' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px', borderBottom: '1px solid #e5e7eb', paddingBottom: '16px' }}>
        <h2 style={{ margin: 0, fontSize: '24px', fontWeight: 700 }}>
          Report Card Details: {report.studentName}
          <span style={{ fontSize: '14px', fontWeight: 400, color: '#6b7280', marginLeft: '12px' }}>
            Admn No: {report.admissionNumber || 'N/A'}
          </span>
        </h2>
        <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: '24px', cursor: 'pointer', color: '#9ca3af' }}>&times;</button>
      </div>

      {error && <ValidationMessage type="error" message={error} />}
      {success && <ValidationMessage type="success" message={success} />}

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', marginBottom: '24px' }}>
        {/* Academic Overview (Read-Only) */}
        <div style={{ backgroundColor: '#f9fafb', padding: '16px', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
          <h3 style={{ marginTop: 0, fontSize: '16px', borderBottom: '1px solid #d1d5db', paddingBottom: '8px' }}>Academic Summary</h3>
          <p style={{ margin: '8px 0', fontSize: '14px' }}><strong>Total Marks:</strong> {report.totalMarksObtained ?? '-'} / {report.totalMarksObtainable ?? '-'}</p>
          <p style={{ margin: '8px 0', fontSize: '14px' }}><strong>Average Score:</strong> {report.averageScore?.toFixed(2) ?? '-'}%</p>
          <p style={{ margin: '8px 0', fontSize: '14px' }}><strong>Position:</strong> {report.positionInClass ?? '-'} out of {report.numberInClass ?? '-'}</p>
          <p style={{ margin: '8px 0', fontSize: '14px' }}><strong>Status:</strong> {['Draft', 'Moderated', 'Approved', 'Published'][report.status]}</p>
        </div>

        {/* Attendance */}
        <div style={{ backgroundColor: '#fff', padding: '16px', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
          <h3 style={{ marginTop: 0, fontSize: '16px', borderBottom: '1px solid #d1d5db', paddingBottom: '8px' }}>Attendance</h3>
          <div style={{ display: 'grid', gap: '12px' }}>
            <div>
              <label style={labelStyle}>Times School Opened</label>
              <input type="number" name="timesSchoolOpened" style={inputStyle} value={formData.timesSchoolOpened || ''} onChange={handleInputChange} />
            </div>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
              <div>
                <label style={labelStyle}>Times Present</label>
                <input type="number" name="timesPresent" style={inputStyle} value={formData.timesPresent || ''} onChange={handleInputChange} />
              </div>
              <div>
                <label style={labelStyle}>Times Absent</label>
                <input type="number" name="timesAbsent" style={inputStyle} value={formData.timesAbsent || ''} onChange={handleInputChange} />
              </div>
            </div>
          </div>
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', marginBottom: '24px' }}>
        {/* Affective Domain */}
        <div style={{ backgroundColor: '#fff', padding: '16px', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
          <h3 style={{ marginTop: 0, fontSize: '16px', borderBottom: '1px solid #d1d5db', paddingBottom: '8px' }}>Affective Domain (A-E)</h3>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <TraitInput label="Regularity" name="regularity" value={formData.regularity} onChange={handleInputChange} />
            <TraitInput label="Punctuality" name="punctuality" value={formData.punctuality} onChange={handleInputChange} />
            <TraitInput label="Neatness" name="neatness" value={formData.neatness} onChange={handleInputChange} />
            <TraitInput label="Attitude" name="attitudeInSchool" value={formData.attitudeInSchool} onChange={handleInputChange} />
            <TraitInput label="Social" name="socialActivities" value={formData.socialActivities} onChange={handleInputChange} />
          </div>
        </div>

        {/* Psychomotor */}
        <div style={{ backgroundColor: '#fff', padding: '16px', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
          <h3 style={{ marginTop: 0, fontSize: '16px', borderBottom: '1px solid #d1d5db', paddingBottom: '8px' }}>Psychomotor Skills (A-E)</h3>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <TraitInput label="Indoor Games" name="indoorGames" value={formData.indoorGames} onChange={handleInputChange} />
            <TraitInput label="Field Games" name="fieldGames" value={formData.fieldGames} onChange={handleInputChange} />
            <TraitInput label="Track Games" name="trackGames" value={formData.trackGames} onChange={handleInputChange} />
            <TraitInput label="Jumps" name="jumps" value={formData.jumps} onChange={handleInputChange} />
            <TraitInput label="Swims" name="swims" value={formData.swims} onChange={handleInputChange} />
          </div>
        </div>
      </div>

      {/* Remarks */}
      <div style={{ backgroundColor: '#fff', padding: '16px', borderRadius: '8px', border: '1px solid #e5e7eb', marginBottom: '24px' }}>
        <h3 style={{ marginTop: 0, fontSize: '16px', borderBottom: '1px solid #d1d5db', paddingBottom: '8px' }}>Teacher Remarks</h3>
        <div style={{ display: 'grid', gap: '16px' }}>
          <div>
            <label style={labelStyle}>Class Teacher's Comment</label>
            <textarea name="classTeacherComment" style={textareaStyle} value={formData.classTeacherComment} onChange={handleInputChange}></textarea>
          </div>
          <div>
            <label style={labelStyle}>Head Teacher's Comment</label>
            <textarea name="headTeacherComment" style={textareaStyle} value={formData.headTeacherComment} onChange={handleInputChange}></textarea>
          </div>
        </div>
      </div>

      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px' }}>
        <button onClick={onClose} style={{ padding: '10px 20px', backgroundColor: '#f3f4f6', color: '#374151', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 600 }}>Cancel</button>
        <button onClick={handleSave} disabled={saving} style={{ padding: '10px 24px', backgroundColor: '#2563eb', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 600 }}>
          {saving ? 'Saving...' : 'Save Report Details'}
        </button>
      </div>
    </div>
  );
};

const TraitInput = ({ label, name, value, onChange }: { label: string, name: string, value: string | undefined, onChange: any }) => (
  <div>
    <label style={{ display: 'block', fontSize: '12px', fontWeight: 600, marginBottom: '4px', color: '#4b5563' }}>{label}</label>
    <input type="text" name={name} style={{...inputStyle, textAlign: 'center'}} value={value || ''} onChange={onChange} maxLength={5} placeholder="e.g. A" />
  </div>
);

const labelStyle: React.CSSProperties = {
  display: 'block',
  fontSize: '13px',
  fontWeight: 600,
  marginBottom: '6px',
  color: '#374151'
};

const inputStyle: React.CSSProperties = {
  width: '100%',
  padding: '8px 12px',
  borderRadius: '6px',
  border: '1px solid #d1d5db',
  fontSize: '14px',
  boxSizing: 'border-box'
};

const textareaStyle: React.CSSProperties = {
  ...inputStyle,
  minHeight: '80px',
  resize: 'vertical'
};

export default ReportCardView;
