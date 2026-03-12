import React, { useState, useEffect, useCallback } from 'react';
import { configurationService } from '../../services/configurationService';
import { AdmissionNumberConfiguration } from '../../models/admission-number-configuration';

interface AdmissionNumberManagementProps {
  admissionConfig: AdmissionNumberConfiguration & { dataLoaded: boolean };
  setAdmissionConfig: React.Dispatch<React.SetStateAction<AdmissionNumberConfiguration & { dataLoaded: boolean }>>;
}

const AdmissionNumberManagement: React.FC<AdmissionNumberManagementProps> = ({ 
  admissionConfig, 
  setAdmissionConfig 
}) => {
  const [originalFormat, setOriginalFormat] = useState<string>('EN-{YYYY}-{0000}');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const loadAdmissionConfig = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const response = await configurationService.getAdmissionNumberConfiguration();
      const config = response.data;
      setAdmissionConfig({ ...config, dataLoaded: true });
      setOriginalFormat(config.format);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to load admission number configuration');
    } finally {
      setLoading(false);
    }
  }, [setAdmissionConfig]);

  useEffect(() => {
    if (admissionConfig.dataLoaded) {
      return;
    }
    loadAdmissionConfig();
  }, [admissionConfig.dataLoaded, loadAdmissionConfig]);

  const handleSave = async () => {
    if (!admissionConfig.format.trim()) {
      setError('Format cannot be empty');
      return;
    }

    setLoading(true);
    setError('');
    setSuccess('');
    try {
      const { dataLoaded, ...optionsToSave } = admissionConfig;
      await configurationService.setAdmissionNumberConfiguration(optionsToSave);
      setOriginalFormat(admissionConfig.format);
      setSuccess('Admission number format saved successfully');
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to save admission number settings');
    } finally {
      setLoading(false);
    }
  };

  const hasUnsavedChanges = () => {
    return admissionConfig.format !== originalFormat;
  };

  const generatePreview = (format: string, sequence: number) => {
    const year = new Date().getFullYear().toString();
    const sequenceStr = (sequence + 1).toString().padStart(4, '0');
    return format.replace('{YYYY}', year).replace('{0000}', sequenceStr);
  };

  const styles = {
    container: {
      padding: '0',
      maxWidth: '800px',
      fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
    } as React.CSSProperties,
    card: {
      backgroundColor: '#ffffff',
      borderRadius: '12px',
      padding: '32px',
      marginBottom: '32px',
      boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
      border: '1px solid #e5e7eb',
    } as React.CSSProperties,
    sectionTitle: {
      fontSize: '20px',
      fontWeight: 600,
      marginBottom: '24px',
      color: '#374151',
    } as React.CSSProperties,
    formGroup: {
      marginBottom: '24px',
    } as React.CSSProperties,
    label: {
      display: 'block',
      fontSize: '14px',
      fontWeight: 500,
      color: '#374151',
      marginBottom: '8px',
    } as React.CSSProperties,
    input: {
      width: '100%',
      padding: '10px 12px',
      border: '1px solid #d1d5db',
      borderRadius: '6px',
      fontSize: '14px',
      boxSizing: 'border-box' as const,
    } as React.CSSProperties,
    helpText: {
      fontSize: '13px',
      color: '#6b7280',
      marginTop: '8px',
      lineHeight: '1.5',
    } as React.CSSProperties,
    previewBox: {
      marginTop: '16px',
      padding: '12px',
      backgroundColor: '#f3f4f6',
      borderRadius: '6px',
      border: '1px solid #e5e7eb',
    } as React.CSSProperties,
    previewTitle: {
      fontSize: '12px',
      fontWeight: 600,
      color: '#6b7280',
      textTransform: 'uppercase' as const,
      marginBottom: '4px',
    } as React.CSSProperties,
    previewValue: {
      fontSize: '16px',
      fontWeight: 600,
      color: '#111827',
      fontFamily: 'monospace',
    } as React.CSSProperties,
    buttonGroup: {
      display: 'flex',
      justifyContent: 'flex-start',
      gap: '12px',
      marginTop: '24px',
    } as React.CSSProperties,
    button: {
      padding: '10px 16px',
      fontSize: '14px',
      fontWeight: 500,
      borderRadius: '6px',
      cursor: 'pointer',
      border: 'none',
      transition: 'all 0.2s',
    } as React.CSSProperties,
    saveButton: {
      backgroundColor: '#2563eb',
      color: 'white',
      opacity: (loading || !hasUnsavedChanges()) ? 0.7 : 1,
      cursor: (loading || !hasUnsavedChanges()) ? 'not-allowed' : 'pointer',
    } as React.CSSProperties,
    errorMessage: {
      backgroundColor: '#fef2f2',
      border: '1px solid #fecaca',
      borderRadius: '8px',
      padding: '12px 16px',
      marginTop: '16px',
      color: '#dc2626',
      fontSize: '14px',
      display: 'flex',
      alignItems: 'center',
      gap: '8px',
    } as React.CSSProperties,
    successMessage: {
      backgroundColor: '#f0fdf4',
      border: '1px solid #bbf7d0',
      borderRadius: '8px',
      padding: '12px 16px',
      marginTop: '16px',
      color: '#059669',
      fontSize: '14px',
      display: 'flex',
      alignItems: 'center',
      gap: '8px',
    } as React.CSSProperties,
    loadingOverlay: {
      position: 'absolute',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(255, 255, 255, 0.8)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      borderRadius: '12px',
    } as React.CSSProperties,
  };

  return (
    <div style={styles.container}>
      <div style={{ position: 'relative' }}>
        <div style={styles.card}>
          <h3 style={styles.sectionTitle}>Admission Number Configuration</h3>
          
          <div style={styles.formGroup}>
            <label style={styles.label}>Admission Number Format</label>
            <input
              type="text"
              value={admissionConfig.format}
              onChange={(e) => setAdmissionConfig({ ...admissionConfig, format: e.target.value })}
              style={styles.input}
              placeholder="EN-{YYYY}-{0000}"
              disabled={loading}
            />
            <div style={styles.helpText}>
              Define the format for auto-generated student admission numbers. 
              <br/>
              Available placeholders: 
              <strong>{'{YYYY}'}</strong> (Current Year), 
              <strong>{'{0000}'}</strong> (Sequential Number - e.g., 0001)
            </div>
            
            <div style={styles.previewBox}>
              <div style={styles.previewTitle}>Live Preview (Next Student)</div>
              <div style={styles.previewValue}>
                {generatePreview(admissionConfig.format || 'EN-{YYYY}-{0000}', admissionConfig.lastSequenceNumber || 0)}
              </div>
            </div>
          </div>

          <div style={styles.buttonGroup}>
            <button
              style={styles.saveButton}
              onClick={handleSave}
              disabled={loading || !hasUnsavedChanges()}
            >
              {loading ? 'Saving...' : 'Save Format'}
            </button>
          </div>

          {error && (
            <div style={styles.errorMessage}>
              <img src="/images/icons/fail.svg" alt="Error" style={{width: '16px', height: '16px'}} />
              {error}
            </div>
          )}

          {success && (
            <div style={styles.successMessage}>
              <img src="/images/icons/check.svg" alt="Success" style={{width: '16px', height: '16px'}} />
              {success}
            </div>
          )}
        </div>

        {loading && (
          <div style={styles.loadingOverlay}>
            <div>Loading...</div>
          </div>
        )}
      </div>
    </div>
  );
};

export default AdmissionNumberManagement;
