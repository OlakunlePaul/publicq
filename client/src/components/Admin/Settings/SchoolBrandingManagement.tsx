import React, { useState, useEffect } from 'react';
import commonStyles from '../AdminCommon.module.css';
import { configurationService } from '../../../services/configurationService';
import { SchoolBrandingConfiguration } from '../../../models/school-branding-configuration';
import { ValidationMessage } from '../../Shared/ValidationComponents';

const SchoolBrandingManagement: React.FC = () => {
    const [config, setConfig] = useState<SchoolBrandingConfiguration>({
        schoolName: '',
        schoolAddress: '',
        schoolPhone: '',
        schoolLogoUrl: ''
    });
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const [logoFile, setLogoFile] = useState<File | null>(null);
    const [uploading, setUploading] = useState(false);

    useEffect(() => {
        const fetchConfig = async () => {
            try {
                const response = await configurationService.getSchoolBrandingConfiguration();
                if (response.isSuccess && response.data) {
                    setConfig(response.data);
                } else {
                    setError('Failed to load branding configuration.');
                }
            } catch (err) {
                setError('An error occurred while fetching configuration.');
            } finally {
                setLoading(false);
            }
        };

        fetchConfig();
    }, []);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setConfig(prev => ({ ...prev, [name]: value }));
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files[0]) {
            setLogoFile(e.target.files[0]);
        }
    };

    const handleUploadLogo = async () => {
        if (!logoFile) return;
        setUploading(true);
        setError(null);
        try {
            const response = await configurationService.uploadSchoolLogo(logoFile);
            if (response.isSuccess && response.data) {
                setConfig(prev => ({ ...prev, schoolLogoUrl: response.data as any }));
                setSuccess('Logo uploaded successfully.');
                setLogoFile(null);
                // The backend returns the URL in data
            } else {
                setError(response.message || 'Failed to upload logo.');
            }
        } catch (err) {
            setError('Error uploading logo.');
        } finally {
            setUploading(false);
        }
    };

    const handleSave = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        setError(null);
        setSuccess(null);

        try {
            const response = await configurationService.setSchoolBrandingConfiguration(config);
            if (response.isSuccess) {
                setSuccess('School branding updated successfully.');
            } else {
                setError('Failed to update formatting.');
            }
        } catch (err) {
            setError('An error occurred while saving.');
        } finally {
            setSaving(false);
        }
    };

    if (loading) {
        return <div className={commonStyles.loading}>Loading configuration...</div>;
    }

    return (
        <div className={commonStyles.formContainer}>
            <div className={commonStyles.formHeader}>
                <h3>School Branding & Customization</h3>
                <p>Configure the school's identity for the Parent Portal and Student Report Cards.</p>
            </div>

            {error && <ValidationMessage type="error" message={error} />}
            {success && <ValidationMessage type="success" message={success} />}

            <form onSubmit={handleSave} className={commonStyles.form}>
                <div className={commonStyles.formGroup}>
                    <label htmlFor="schoolName" className={commonStyles.label}>School Name</label>
                    <input
                        id="schoolName"
                        name="schoolName"
                        type="text"
                        value={config.schoolName}
                        onChange={handleChange}
                        className={commonStyles.input}
                        required
                        placeholder="e.g. Day & Boarding School"
                    />
                </div>

                <div className={commonStyles.formGroup}>
                    <label htmlFor="schoolAddress" className={commonStyles.label}>School Address</label>
                    <input
                        id="schoolAddress"
                        name="schoolAddress"
                        type="text"
                        value={config.schoolAddress}
                        onChange={handleChange}
                        className={commonStyles.input}
                        placeholder="e.g. 123 Education Avenue"
                    />
                </div>

                <div className={commonStyles.formGroup}>
                    <label htmlFor="schoolPhone" className={commonStyles.label}>Telephone Number</label>
                    <input
                        id="schoolPhone"
                        name="schoolPhone"
                        type="text"
                        value={config.schoolPhone}
                        onChange={handleChange}
                        className={commonStyles.input}
                        placeholder="e.g. 0800-SCHOOL"
                    />
                </div>

                <div className={commonStyles.formGroup}>
                    <label className={commonStyles.label}>School Logo</label>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '20px', marginBottom: '10px' }}>
                        {config.schoolLogoUrl && (
                            <div style={{ 
                                width: '100px', 
                                height: '100px', 
                                border: '1px solid #e5e7eb', 
                                borderRadius: '8px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                background: '#f9fafb',
                                overflow: 'hidden'
                            }}>
                                <img 
                                    src={config.schoolLogoUrl} 
                                    alt="School Logo" 
                                    style={{ maxWidth: '100%', maxHeight: '100%', objectFit: 'contain' }} 
                                />
                            </div>
                        )}
                        <div style={{ flex: 1 }}>
                            <input
                                id="logoUpload"
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                                className={commonStyles.input}
                                style={{ marginBottom: '8px' }}
                            />
                            <button
                                type="button"
                                onClick={handleUploadLogo}
                                disabled={!logoFile || uploading}
                                className={commonStyles.submitButton}
                                style={{ width: 'auto', padding: '0.5rem 1rem', fontSize: '0.875rem', backgroundColor: '#4f46e5' }}
                            >
                                {uploading ? 'Uploading...' : 'Upload Logo'}
                            </button>
                        </div>
                    </div>
                </div>

                <div className={commonStyles.formGroup}>
                    <label htmlFor="schoolLogoUrl" className={commonStyles.label}>Logo URL (Manual Entry)</label>
                    <input
                        id="schoolLogoUrl"
                        name="schoolLogoUrl"
                        type="url"
                        value={config.schoolLogoUrl || ''}
                        onChange={handleChange}
                        className={commonStyles.input}
                        placeholder="https://example.com/logo.png"
                    />
                    <small style={{display: 'block', marginTop: '4px', color: '#6b7280'}}>
                        Upload a logo above or provide a direct URL to the image.
                    </small>
                </div>

                <div className={commonStyles.formActions}>
                    <button
                        type="submit"
                        disabled={saving}
                        className={commonStyles.submitButton}
                    >
                        {saving ? 'Saving...' : 'Save Branding'}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default SchoolBrandingManagement;
