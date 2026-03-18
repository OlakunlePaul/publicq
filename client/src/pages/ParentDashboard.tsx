import React, { useState, useEffect } from 'react';
import commonStyles from '../Admin/AdminCommon.module.css';
import { resultService, StudentAssessment } from '../services/resultService';
import { ValidationMessage } from '../components/Shared/ValidationComponents';
import ReportCardView from '../components/ResultManagement/ReportCardView';

const ParentDashboard: React.FC = () => {
  const [results, setResults] = useState<StudentAssessment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedAssessmentId, setSelectedAssessmentId] = useState<string | null>(null);

  useEffect(() => {
    const fetchResults = async () => {
      try {
        const data = await resultService.getParentChildrenResults();
        setResults(data);
      } catch (err: any) {
        setError('Failed to load children results: ' + (err.response?.data?.message || err.message));
      } finally {
        setLoading(false);
      }
    };

    fetchResults();
  }, []);

  if (loading) return <div style={{ padding: '40px', textAlign: 'center' }}>Loading your children's results...</div>;

  return (
    <div className={commonStyles.container} style={{ maxWidth: '1000px', margin: '0 auto', padding: '40px 20px' }}>
      <header style={{ marginBottom: '32px' }}>
        <h1 className={commonStyles.title} style={{ fontSize: '28px' }}>Parent Dashboard</h1>
        <p className={commonStyles.description}>
          View and track your children's academic performance and report cards.
        </p>
      </header>

      {error && <ValidationMessage type="error" message={error} />}

      {results.length === 0 && !error ? (
        <div className={commonStyles.card} style={{ padding: '40px', textAlign: 'center', color: '#6b7280' }}>
            <p style={{ fontSize: '18px', marginBottom: '8px' }}>No results found.</p>
            <p style={{ fontSize: '14px' }}>If you believe this is an error, please contact the school administration to link your account to your children's profiles.</p>
        </div>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '24px' }}>
          {results.map(res => (
            <div key={res.id} className={commonStyles.card} style={{ position: 'relative', overflow: 'hidden' }}>
              <div style={cardHeaderStyle}>
                <h3 style={{ margin: 0, fontSize: '18px', color: '#111827' }}>{res.studentName}</h3>
                <span style={{ fontSize: '12px', color: '#6b7280' }}>ID: {res.admissionNumber}</span>
              </div>
              
              <div style={{ padding: '20px' }}>
                <div style={infoRowStyle}>
                  <span style={labelStyle}>Session:</span>
                  <span style={valueStyle}>{res.sessionName}</span>
                </div>
                <div style={infoRowStyle}>
                  <span style={labelStyle}>Term:</span>
                  <span style={valueStyle}>{res.termName}</span>
                </div>
                <div style={infoRowStyle}>
                  <span style={labelStyle}>Class:</span>
                  <span style={valueStyle}>{res.className}</span>
                </div>
                <div style={{ ...infoRowStyle, borderBottom: 'none', marginBottom: '20px' }}>
                  <span style={labelStyle}>Average:</span>
                  <span style={{ ...valueStyle, fontWeight: 700, color: '#3b82f6' }}>{res.averageScore?.toFixed(1) || 'N/A'}%</span>
                </div>

                <button 
                  onClick={() => setSelectedAssessmentId(res.id)}
                  style={viewButtonStyle}
                >
                  View Full Report Card
                </button>
              </div>

              {res.status !== 3 && ( // 3 = Published
                <div style={overlayStyle}>
                    <div style={badgeStyle}>Processing</div>
                </div>
              )}
            </div>
          ))}
        </div>
      )}

      {selectedAssessmentId && (
        <div style={modalOverlayStyle}>
          <ReportCardView 
            assessmentId={selectedAssessmentId} 
            onClose={() => setSelectedAssessmentId(null)}
            onSaved={() => setSelectedAssessmentId(null)}
            readOnly={true}
          />
        </div>
      )}
    </div>
  );
};

const cardHeaderStyle: React.CSSProperties = {
  padding: '20px',
  backgroundColor: '#f9fafb',
  borderBottom: '1px solid #e5e7eb',
};

const infoRowStyle: React.CSSProperties = {
  display: 'flex',
  justifyContent: 'space-between',
  padding: '8px 0',
  borderBottom: '1px solid #f3f4f6',
  fontSize: '14px'
};

const labelStyle: React.CSSProperties = {
  color: '#6b7280'
};

const valueStyle: React.CSSProperties = {
  color: '#374151',
  fontWeight: 500
};

const viewButtonStyle: React.CSSProperties = {
  width: '100%',
  padding: '12px',
  backgroundColor: '#3b82f6',
  color: 'white',
  border: 'none',
  borderRadius: '8px',
  fontWeight: 600,
  cursor: 'pointer',
  transition: 'background-color 0.2s',
};

const overlayStyle: React.CSSProperties = {
    position: 'absolute',
    top: 0, left: 0, right: 0, bottom: 0,
    backgroundColor: 'rgba(255, 255, 255, 0.7)',
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    zIndex: 5
};

const badgeStyle: React.CSSProperties = {
    padding: '6px 12px',
    backgroundColor: '#f59e0b',
    color: 'white',
    borderRadius: '16px',
    fontSize: '12px',
    fontWeight: 600
};

const modalOverlayStyle: React.CSSProperties = {
  position: 'fixed',
  top: 0, left: 0, right: 0, bottom: 0,
  backgroundColor: 'rgba(0,0,0,0.6)',
  display: 'flex',
  justifyContent: 'center',
  alignItems: 'center',
  zIndex: 1000,
  padding: '20px'
};

export default ParentDashboard;
