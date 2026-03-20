import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { getUserInformation, type CurrentUser } from '../../utils/tokenUtils';
import { cn } from '../../utils/cn';
import styles from './ProfileSettingsModal.module.css';

interface ProfileSettingsModalProps {
  onClose: () => void;
}

const ProfileSettingsModal: React.FC<ProfileSettingsModalProps> = ({ onClose }) => {
  const { userRoles } = useAuth();
  const [user, setUser] = useState<CurrentUser | null>(null);

  useEffect(() => {
    try {
      const userInfo = getUserInformation();
      setUser(userInfo);
    } catch (e) {
      console.error('Failed to parse user info', e);
    }
  }, []);

  if (!user) return null;

  return (
    <div className={styles.overlay} onClick={onClose}>
      <div className={styles.modalContainer} onClick={(e) => e.stopPropagation()}>
        <div className={styles.header}>
          <h2 className={styles.title}>Profile Settings</h2>
          <button className={styles.closeBtn} onClick={onClose} aria-label="Close">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M18 6L6 18M6 6l12 12" />
            </svg>
          </button>
        </div>
        
        <div className={styles.contentArea}>
          {/* User Info Section */}
          <div className={styles.section}>
            <h3 className={styles.sectionTitle}>Account Details</h3>
            <div className={styles.detailsCard}>
              <div className={styles.detailRow}>
                <span className={styles.detailLabel}>Name</span>
                <span className={styles.detailValue}>{user.fullName || 'N/A'}</span>
              </div>
              <div className={styles.detailRow}>
                <span className={styles.detailLabel}>Email</span>
                <span className={styles.detailValue}>{user.email || 'N/A'}</span>
              </div>
              <div className={styles.detailRow}>
                <span className={styles.detailLabel}>Roles</span>
                <div className={styles.badgesWrapper}>
                  {userRoles.map(role => (
                    <span key={role} className={styles.roleBadge}>{role}</span>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfileSettingsModal;
