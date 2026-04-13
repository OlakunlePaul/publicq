import React, { useState, useEffect } from 'react';
import commonStyles from '../AdminCommon.module.css';
import { configurationService } from '../../../services/configurationService';
import { S3Configuration } from '../../../models/s3-configuration';
import { ValidationMessage } from '../../Shared/ValidationComponents';

const S3StorageManagement: React.FC = () => {
    const [config, setConfig] = useState<S3Configuration>({
        enabled: false,
        accessKey: '',
        secretKey: '',
        bucketName: '',
        endpoint: '',
        serviceUrl: '',
        region: ''
    });
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    useEffect(() => {
        const fetchConfig = async () => {
            try {
                const response = await configurationService.getS3Options();
                if (response.isSuccess && response.data) {
                    setConfig(response.data);
                } else {
                    setError('Failed to load S3 configuration.');
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
        const { name, value, type, checked } = e.target;
        setConfig(prev => ({ 
            ...prev, 
            [name]: type === 'checkbox' ? checked : value 
        }));
    };

    const handleSave = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        setError(null);
        setSuccess(null);

        try {
            const response = await configurationService.setS3Options(config);
            if (response.isSuccess) {
                setSuccess('S3 storage configuration updated successfully.');
            } else {
                setError(response.message || 'Failed to update configuration.');
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
                <h3>Cloud Storage Settings (Railway Buckets / S3)</h3>
                <p>Configure persistent object storage for file uploads. This is required for Railway deployments to ensure files are not lost on redeploy.</p>
            </div>

            {error && <ValidationMessage type="error" message={error} />}
            {success && <ValidationMessage type="success" message={success} />}

            <form onSubmit={handleSave} className={commonStyles.form}>
                <div className={commonStyles.formGroup} style={{ flexDirection: 'row', alignItems: 'center', gap: '10px' }}>
                    <input
                        id="enabled"
                        name="enabled"
                        type="checkbox"
                        checked={config.enabled}
                        onChange={handleChange}
                        style={{ width: 'auto' }}
                    />
                    <label htmlFor="enabled" className={commonStyles.label} style={{ marginBottom: 0 }}>Enable S3-Compatible Storage (e.g. Railway Buckets)</label>
                </div>

                <div className={commonStyles.formGroup}>
                    <label htmlFor="accessKey" className={commonStyles.label}>Access Key</label>
                    <input
                        id="accessKey"
                        name="accessKey"
                        type="text"
                        value={config.accessKey}
                        onChange={handleChange}
                        className={commonStyles.input}
                        placeholder="Your Access Key"
                    />
                </div>

                <div className={commonStyles.formGroup}>
                    <label htmlFor="secretKey" className={commonStyles.label}>Secret Key</label>
                    <input
                        id="secretKey"
                        name="secretKey"
                        type="password"
                        value={config.secretKey}
                        onChange={handleChange}
                        className={commonStyles.input}
                        placeholder="Your Secret Key"
                    />
                </div>

                <div className={commonStyles.formGroup}>
                    <label htmlFor="bucketName" className={commonStyles.label}>Bucket Name</label>
                    <input
                        id="bucketName"
                        name="bucketName"
                        type="text"
                        value={config.bucketName}
                        onChange={handleChange}
                        className={commonStyles.input}
                        placeholder="my-publicq-bucket"
                    />
                </div>

                <div className={commonStyles.formGroup}>
                    <label htmlFor="endpoint" className={commonStyles.label}>Endpoint / Service URL</label>
                    <input
                        id="endpoint"
                        name="endpoint"
                        type="text"
                        value={config.endpoint}
                        onChange={handleChange}
                        className={commonStyles.input}
                        placeholder="e.g. https://s3.railway.app"
                    />
                </div>

                <div className={commonStyles.formGroup}>
                    <label htmlFor="region" className={commonStyles.label}>Region (Optional)</label>
                    <input
                        id="region"
                        name="region"
                        type="text"
                        value={config.region}
                        onChange={handleChange}
                        className={commonStyles.input}
                        placeholder="e.g. us-east-1"
                    />
                </div>

                <div className={commonStyles.formActions}>
                    <button
                        type="submit"
                        disabled={saving}
                        className={commonStyles.submitButton}
                    >
                        {saving ? 'Saving...' : 'Save Storage Settings'}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default S3StorageManagement;
