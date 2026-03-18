import React, { useState } from 'react';
import { resultService, ResultUploadResponse } from '../../services/resultService';
import { ValidationMessage } from '../Shared/ValidationComponents';

interface ResultUploadProps {
  sessionId: string;
  termId: string;
  classLevelId: string;
  onSuccess: () => void;
  onClose: () => void;
}

const ResultUpload: React.FC<ResultUploadProps> = ({ 
  sessionId, termId, classLevelId, onSuccess, onClose 
}) => {
  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState('');
  const [uploadResult, setUploadResult] = useState<ResultUploadResponse | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setFile(e.target.files[0]);
      setError('');
      setUploadResult(null);
    }
  };

  const handleUpload = async () => {
    if (!file) {
      setError('Please select a CSV file.');
      return;
    }

    setUploading(true);
    setError('');
    setUploadResult(null);

    try {
      const result = await resultService.uploadCsv(file, sessionId, termId, classLevelId);
      setUploadResult(result);
      
      if (result.successCount > 0) {
        // Wait 2 seconds then notify parent and close
        setTimeout(() => {
            onSuccess();
            onClose();
        }, 2000);
      }
    } catch (err: any) {
      setError(err.response?.data?.errors?.[0] || 'Failed to upload result.');
    } finally {
      setUploading(false);
    }
  };

  return (
    <div style={containerStyle}>
      <div style={headerStyle}>
        <h3 style={{ margin: 0 }}>Bulk Upload Results (CSV)</h3>
        <button onClick={onClose} style={closeButtonStyle}>×</button>
      </div>
      
      <div style={contentStyle}>
        <p style={{ fontSize: '14px', color: '#4b5563', marginBottom: '20px' }}>
          Select a CSV file in the "Mercy's Gate" Report Card format. 
          The system will automatically match students by name and import scores, comments, and attendance.
        </p>

        <div style={uploadBoxStyle}>
          <input 
            type="file" 
            accept=".csv" 
            onChange={handleFileChange}
            style={{ marginBottom: '16px' }}
          />
          {file && (
            <div style={{ fontSize: '14px', marginBottom: '16px' }}>
                Selected: <strong>{file.name}</strong> ({(file.size / 1024).toFixed(1)} KB)
            </div>
          )}
          
          <button 
            onClick={handleUpload}
            disabled={!file || uploading}
            style={{
                ...uploadButtonStyle,
                backgroundColor: !file || uploading ? '#93c5fd' : '#3b82f6'
            }}
          >
            {uploading ? 'Processing File...' : 'Upload & Process CSV'}
          </button>
        </div>

        {error && <ValidationMessage type="error" message={error} />}
        
        {uploadResult && (
          <div style={resultStyle}>
            <div style={{ fontWeight: 600, marginBottom: '8px' }}>Upload Summary:</div>
            <ul style={{ margin: 0, paddingLeft: '20px', fontSize: '14px' }}>
                <li style={{ color: '#10b981' }}>Successfully Processed: {uploadResult.successCount}</li>
                <li style={{ color: uploadResult.failureCount > 0 ? '#ef4444' : '#6b7280' }}>Failures: {uploadResult.failureCount}</li>
            </ul>
            {uploadResult.errors.length > 0 && (
              <div style={{ marginTop: '12px', fontSize: '12px', color: '#ef4444' }}>
                <strong>Details:</strong>
                {uploadResult.errors.map((err, i) => <div key={i}>• {err}</div>)}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

const containerStyle: React.CSSProperties = {
  backgroundColor: 'white',
  borderRadius: '12px',
  width: '100%',
  maxWidth: '500px',
  boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
  overflow: 'hidden'
};

const headerStyle: React.CSSProperties = {
  padding: '16px 20px',
  borderBottom: '1px solid #e5e7eb',
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  backgroundColor: '#f9fafb'
};

const contentStyle: React.CSSProperties = {
  padding: '20px'
};

const uploadBoxStyle: React.CSSProperties = {
  border: '2px dashed #d1d5db',
  borderRadius: '8px',
  padding: '24px',
  textAlign: 'center',
  marginBottom: '20px',
  backgroundColor: '#f9fafb'
};

const uploadButtonStyle: React.CSSProperties = {
  padding: '10px 24px',
  color: 'white',
  border: 'none',
  borderRadius: '6px',
  fontWeight: 600,
  cursor: 'pointer',
  transition: 'background-color 0.2s'
};

const closeButtonStyle: React.CSSProperties = {
  background: 'none',
  border: 'none',
  fontSize: '24px',
  cursor: 'pointer',
  color: '#9ca3af'
};

const resultStyle: React.CSSProperties = {
  padding: '16px',
  backgroundColor: '#f3f4f6',
  borderRadius: '8px',
  marginTop: '16px'
};

export default ResultUpload;
