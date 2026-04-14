import React, { useState, useEffect, useRef } from 'react';
import styles from './SchoolBranding.module.css';
import { configurationService } from '../../../services/configurationService';
import { SchoolBrandingConfiguration } from '../../../models/school-branding-configuration';
import { ValidationMessage } from '../../Shared/ValidationComponents';
import { config as appConfig } from '../../../config';
import { motion, AnimatePresence, Variants } from 'framer-motion';
import { 
    School, 
    MapPin, 
    Phone, 
    Upload, 
    Save, 
    ShieldCheck, 
    Camera, 
    Globe, 
    Loader2,
    CheckCircle2
} from 'lucide-react';

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
    const fileInputRef = useRef<HTMLInputElement>(null);

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
            // Auto-trigger upload for better UX? Or wait for click?
            // Let's wait for click but show the file is ready.
        }
    };

    const handleSaveInternal = async (updatedConfig: SchoolBrandingConfiguration) => {
        setSaving(true);
        setError(null);
        try {
            const response = await configurationService.setSchoolBrandingConfiguration(updatedConfig);
            if (response.isSuccess) {
                setSuccess('School branding updated successfully.');
                setTimeout(() => setSuccess(null), 3000);
                return true;
            } else {
                setError(response.message || 'Failed to update configuration.');
                return false;
            }
        } catch (err) {
            setError('An error occurred while saving.');
            return false;
        } finally {
            setSaving(false);
        }
    };

    const handleUploadLogo = async () => {
        if (!logoFile) return;
        setUploading(true);
        setError(null);
        setSuccess(null);
        try {
            const response = await configurationService.uploadSchoolLogo(logoFile);
            if (response.isSuccess && response.data) {
                const newLogoUrl = response.data;
                const newConfig = { ...config, schoolLogoUrl: newLogoUrl };
                setConfig(newConfig);
                setLogoFile(null);
                
                // CRITICAL: Auto-save to persistence storage 
                // to fix the "disappears on refresh" issue
                await handleSaveInternal(newConfig);
                setSuccess('Logo uploaded and saved successfully!');
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
        await handleSaveInternal(config);
    };

    const containerVariants: Variants = {
        hidden: { opacity: 0 },
        visible: {
            opacity: 1,
            transition: {
                staggerChildren: 0.1
            }
        }
    };

    const itemVariants: Variants = {
        hidden: { y: 20, opacity: 0 },
        visible: {
            y: 0,
            opacity: 1,
            transition: {
                type: 'spring',
                stiffness: 300,
                damping: 24
            }
        }
    };

    if (loading) {
        return (
            <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '300px' }}>
                <Loader2 className="animate-spin" size={48} color="#4f46e5" />
            </div>
        );
    }

    const previewUrl = config.schoolLogoUrl 
        ? (config.schoolLogoUrl.startsWith('http') 
            ? config.schoolLogoUrl 
            : `${appConfig.apiBaseUrl}${config.schoolLogoUrl.startsWith('/') ? config.schoolLogoUrl.substring(1) : config.schoolLogoUrl}`)
        : null;

    return (
        <motion.div 
            className={styles.container}
            variants={containerVariants}
            initial="hidden"
            animate="visible"
        >
            <motion.div variants={itemVariants} className={styles.sectionHeader}>
                <h3><School size={24} className="text-indigo-600" /> School Profile</h3>
                <p>Manage your school's public identity, branding assets, and contact details.</p>
            </motion.div>

            <AnimatePresence mode="wait">
                {error && (
                    <motion.div 
                        initial={{ opacity: 0, height: 0 }} 
                        animate={{ opacity: 1, height: 'auto' }} 
                        exit={{ opacity: 0, height: 0 }}
                    >
                        <ValidationMessage type="error" message={error} />
                    </motion.div>
                )}
                {success && (
                    <motion.div 
                        initial={{ opacity: 0, height: 0 }} 
                        animate={{ opacity: 1, height: 'auto' }} 
                        exit={{ opacity: 0, height: 0 }}
                    >
                        <div style={{ padding: '12px', background: '#ecfdf5', color: '#065f46', borderRadius: '8px', marginBottom: '16px', display: 'flex', alignItems: 'center', gap: '8px', fontWeight: 600 }}>
                            <CheckCircle2 size={18} /> {success}
                        </div>
                    </motion.div>
                )}
            </AnimatePresence>

            <div className={styles.grid}>
                {/* 1. Identity Form */}
                <motion.div variants={itemVariants} className={styles.profileCard}>
                    <div className={styles.sectionHeader}>
                        <h3>General Information</h3>
                        <p>Basic contact details used in headers and reports.</p>
                    </div>

                    <form onSubmit={handleSave}>
                        <div className={styles.formGroup}>
                            <label className={styles.label} htmlFor="schoolName">School Name</label>
                            <div style={{ position: 'relative' }}>
                                <input
                                    id="schoolName"
                                    name="schoolName"
                                    type="text"
                                    value={config.schoolName}
                                    onChange={handleChange}
                                    className={styles.input}
                                    placeholder="e.g. Dayspring Intl Schools"
                                    required
                                />
                                <ShieldCheck size={18} style={{ position: 'absolute', right: '12px', top: '50%', transform: 'translateY(-50%)', color: '#9ca3af' }} />
                            </div>
                        </div>

                        <div className={styles.formGroup}>
                            <label className={styles.label} htmlFor="schoolAddress">Campus Address</label>
                            <div style={{ position: 'relative' }}>
                                <input
                                    id="schoolAddress"
                                    name="schoolAddress"
                                    type="text"
                                    value={config.schoolAddress}
                                    onChange={handleChange}
                                    className={styles.input}
                                    placeholder="Enter physical address"
                                />
                                <MapPin size={18} style={{ position: 'absolute', right: '12px', top: '50%', transform: 'translateY(-50%)', color: '#9ca3af' }} />
                            </div>
                        </div>

                        <div className={styles.formGroup}>
                            <label className={styles.label} htmlFor="schoolPhone">Official Telephone</label>
                            <div style={{ position: 'relative' }}>
                                <input
                                    id="schoolPhone"
                                    name="schoolPhone"
                                    type="text"
                                    value={config.schoolPhone}
                                    onChange={handleChange}
                                    className={styles.input}
                                    placeholder="+234..."
                                />
                                <Phone size={18} style={{ position: 'absolute', right: '12px', top: '50%', transform: 'translateY(-50%)', color: '#9ca3af' }} />
                            </div>
                        </div>

                        <button 
                            type="submit" 
                            disabled={saving} 
                            className={styles.saveButton}
                        >
                            {saving ? <Loader2 className="animate-spin" size={18} /> : <Save size={18} />}
                            {saving ? 'Updating Profile...' : 'Save General Details'}
                        </button>
                    </form>
                </motion.div>

                {/* 2. Logo & Branding */}
                <motion.div variants={itemVariants} className={styles.profileCard}>
                    <div className={styles.sectionHeader}>
                        <h3>Official Logo</h3>
                        <p>Display your school identity on all report cards.</p>
                    </div>

                    <div className={styles.logoSection}>
                        <div 
                            className={styles.logoPreviewContainer}
                            onClick={() => fileInputRef.current?.click()}
                        >
                            {previewUrl ? (
                                <img src={previewUrl} alt="Preview" className={styles.logoImage} />
                            ) : (
                                <Camera size={48} color="#d1d5db" />
                            )}
                            <div className={styles.uploadOverlay}>
                                <Upload size={14} style={{ display: 'inline', marginRight: '4px' }} /> Change Logo
                            </div>
                        </div>

                        <div className={styles.uploadZone}>
                            <input
                                ref={fileInputRef}
                                id="logoUpload"
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                                style={{ display: 'none' }}
                            />
                            
                            {logoFile && (
                                <motion.div 
                                    initial={{ opacity: 0, scale: 0.95 }}
                                    animate={{ opacity: 1, scale: 1 }}
                                    style={{ textAlign: 'center', marginBottom: '16px' }}
                                >
                                    <p style={{ fontSize: '0.875rem', fontWeight: 600, color: '#4f46e5' }}>
                                        Ready: {logoFile.name}
                                    </p>
                                    <button
                                        type="button"
                                        onClick={handleUploadLogo}
                                        disabled={uploading}
                                        className={styles.saveButton}
                                        style={{ marginTop: '8px' }}
                                    >
                                        {uploading ? <Loader2 className="animate-spin" size={18} /> : <Upload size={18} />}
                                        {uploading ? 'Processing...' : 'Upload & Persist Logo'}
                                    </button>
                                </motion.div>
                            )}

                            {!logoFile && (
                                <div 
                                    className={styles.dropZone}
                                    onClick={() => fileInputRef.current?.click()}
                                >
                                    <p style={{ fontSize: '0.875rem', color: '#6b7280' }}>
                                        Click logo area to upload or <span style={{ color: '#4f46e5', fontWeight: 600 }}>browse files</span>
                                    </p>
                                </div>
                            )}

                            <div style={{ marginTop: '24px' }}>
                                <label className={styles.label} htmlFor="schoolLogoUrl">External Website URL</label>
                                <div style={{ position: 'relative' }}>
                                    <input
                                        id="schoolLogoUrl"
                                        name="schoolLogoUrl"
                                        type="url"
                                        value={config.schoolLogoUrl || ''}
                                        onChange={handleChange}
                                        className={styles.input}
                                        placeholder="https://your-school.com/logo.png"
                                    />
                                    <Globe size={18} style={{ position: 'absolute', right: '12px', top: '50%', transform: 'translateY(-50%)', color: '#9ca3af' }} />
                                </div>
                                <p style={{ fontSize: '0.75rem', color: '#9ca3af', marginTop: '0.5rem' }}>
                                    Paste a link if your logo is hosted on an external website.
                                </p>
                            </div>
                        </div>
                    </div>
                </motion.div>
            </div>
            
            <motion.div variants={itemVariants} style={{ marginTop: '2rem', display: 'flex', justifyContent: 'center' }}>
                 <p style={{ fontSize: '0.875rem', color: '#9ca3af', display: 'flex', alignItems: 'center', gap: '4px' }}>
                    <ShieldCheck size={14} /> Changes are automatically synced with the Parent Portal.
                 </p>
            </motion.div>
        </motion.div>
    );
};

export default SchoolBrandingManagement;
