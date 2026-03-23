import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './ParentDashboard.module.css';
import { ROUTES } from '../constants/contstants';
import { resultService, StudentAssessment } from '../services/resultService';
import { ValidationMessage } from '../components/Shared/ValidationComponents';
import ReportCardView from '../components/ResultManagement/ReportCardView';

const ParentDashboard: React.FC = () => {
  const navigate = useNavigate();
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

  if (loading) {
    return (
      <div className={styles.container}>
        <header className={styles.header}>
          <h1 className={styles.title}>Parent Dashboard</h1>
          <div style={{ height: '20px', width: '200px', backgroundColor: '#e2e8f0', borderRadius: '4px', animation: 'pulse 2s infinite' }}></div>
        </header>
        <div className={styles.resultsGrid}>
          {[1, 2, 3].map(i => (
            <div key={i} className={styles.skeletonCard}></div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <h1 className={styles.title}>Parent Dashboard</h1>
        <p className={styles.description}>
          View and track your children's academic performance and report cards.
        </p>
      </header>

      <div className={styles.content}>
        {error && <ValidationMessage type="error" message={error} />}

        {results.length === 0 && !error ? (
          <div className={styles.emptyContainer}>
              <p className={styles.emptyMessage}>No results found.</p>
              <p style={{ fontSize: '14px', marginTop: '8px' }}>If you believe this is an error, please contact the school administration to link your account to your children's profiles.</p>
          </div>
        ) : (
          <div className={styles.resultsGrid}>
            {results.map(res => (
              <div key={res.id} className={styles.childCard}>
                <div className={styles.cardHeader}>
                  <h3 className={styles.studentName}>{res.studentName}</h3>
                  <span className={styles.studentId}>ID: {res.admissionNumber}</span>
                </div>
                
                <div className={styles.cardBody}>
                  <div className={styles.infoRow}>
                    <span className={styles.label}>Session:</span>
                    <span className={styles.value}>{res.sessionName}</span>
                  </div>
                  <div className={styles.infoRow}>
                    <span className={styles.label}>Term:</span>
                    <span className={styles.value}>{res.termName}</span>
                  </div>
                  <div className={styles.infoRow}>
                    <span className={styles.label}>Class:</span>
                    <span className={styles.value}>{res.className}</span>
                  </div>
                  <div className={styles.infoRow}>
                    <span className={styles.label}>Average:</span>
                    <span className={`${styles.value} ${styles.averageValue}`}>{res.averageScore?.toFixed(1) || 'N/A'}%</span>
                  </div>

                  <button 
                    onClick={() => setSelectedAssessmentId(res.id)}
                    className={styles.viewButton}
                  >
                    View Full Report Card
                  </button>
                </div>

                {res.status !== 3 && ( // 3 = Published
                  <div className={styles.overlay}>
                      <div className={styles.badge}>Processing</div>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>

      {selectedAssessmentId && (
        <div style={{ position: 'fixed', inset: 0, backgroundColor: 'rgba(0,0,0,0.6)', display: 'flex', justifyContent: 'center', alignItems: 'center', zIndex: 1000, padding: '20px' }}>
          <ReportCardView 
            assessmentId={selectedAssessmentId} 
            onClose={() => setSelectedAssessmentId(null)}
            onSaved={() => setSelectedAssessmentId(null)}
            readOnly={true}
          />
        </div>
      )}

      {/* Mobile Bottom Navigation */}
      <nav className={styles.bottomNav}>
        <button 
          className={`${styles.bottomNavItem} ${styles.active}`}
          onClick={() => navigate(ROUTES.PARENT_DASHBOARD)}
        >
          <img src="https://cdn-icons-png.flaticon.com/512/2641/2641409.png" alt="Results" className={styles.bottomNavIcon} />
          <span className={styles.bottomNavLabel}>Results</span>
        </button>
        
        <button 
          className={styles.bottomNavItem}
          onClick={() => navigate(ROUTES.HOME)}
        >
          <img src="https://cdn-icons-png.flaticon.com/512/1946/1946436.png" alt="Home" className={styles.bottomNavIcon} />
          <span className={styles.bottomNavLabel}>Home</span>
        </button>

        <button 
          className={styles.bottomNavItem}
          onClick={() => navigate(ROUTES.CONTACT_US)}
        >
          <img src="https://cdn-icons-png.flaticon.com/512/9371/9371842.png" alt="Support" className={styles.bottomNavIcon} />
          <span className={styles.bottomNavLabel}>Support</span>
        </button>
      </nav>
      
      <style>{`
        @keyframes pulse {
          0%, 100% { opacity: 1; }
          50% { opacity: .5; }
        }
      `}</style>
    </div>
  );
};

export default ParentDashboard;
