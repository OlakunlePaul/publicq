import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AssignmentAccess from '../components/AssignmentAccess/AssignmentAccess';
import { Assignment } from '../models/assignment';
import { getUserInformation, type CurrentUser } from '../utils/tokenUtils';
import { CONSTANTS, ROUTES } from '../constants/contstants';
import { StudentState } from '../models/student-state';
import styles from './MyExams.module.css';
import { AnimatePresence } from 'framer-motion';
import HandbookModal from '../components/Shared/HandbookModal';

const MyAssignments: React.FC = () => {
  const navigate = useNavigate();
  const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
  const [isHandbookModalOpen, setIsHandbookModalOpen] = useState(false);
  const [activeTab, setActiveTab] = useState<'overview' | 'exams' | 'results' | 'profile'>('exams');

  useEffect(() => {
    // First, try to get authenticated user from token
    try {
      const user = getUserInformation();
      setCurrentUser(user);
    } catch (error) {
      // No token or invalid token, continue to check localStorage
    }
    
    // Check for exam taker in localStorage
    try {
      const examTakerFromStorageJson = localStorage.getItem(CONSTANTS.EXAM_TAKER);
      if (examTakerFromStorageJson) {
        const examTakerFromStorage = JSON.parse(examTakerFromStorageJson) as StudentState;
        
        if (examTakerFromStorage && examTakerFromStorage.id) {
          setCurrentUser(examTakerFromStorage);
        }
      }
    } catch (storageError) {
      // Error parsing exam taker data from localStorage
    }
  }, []);
  
  const handleLoginRequest = () => {
    navigate(`${ROUTES.LOGIN}?redirectTo=${ROUTES.MY_EXAMS}`);
  };

  const handleAssignmentOpen = (assignment: Assignment) => {
    // Navigate to the dedicated assignment execution page
    navigate(`/exam/${assignment.id}`);
  };

  // Function to refresh user info - can be called when exam taker logs in
  const refreshUserInfo = () => {
    try {
      const user = getUserInformation();
      setCurrentUser(user);
    } catch (error) {
      try {
        const examTakerFromStorageJson = localStorage.getItem(CONSTANTS.EXAM_TAKER);
        if (examTakerFromStorageJson) {
          const examTakerFromStorage = JSON.parse(examTakerFromStorageJson) as StudentState;
          if (examTakerFromStorage && examTakerFromStorage.id) {
            setCurrentUser(examTakerFromStorage);
          }
        }
      } catch (storageError) {
        // Error parsing exam taker data from localStorage
      }
    }
  };

  const renderContent = () => {
    switch (activeTab) {
      case 'overview':
        return (
          <div className={styles.tabContent}>
            <div className={styles.welcomeBanner}>
              <h2>Hello, {currentUser?.fullName || 'Student'}! 👋</h2>
              <p>Welcome to your ExamNova dashboard. Use the navigation below to access your exams and results.</p>
            </div>
            
            <div className={styles.quickStats}>
              <div className={styles.statBox}>
                <span className={styles.statLabel}>Active Exams</span>
                <span className={styles.statValue}>-</span>
              </div>
              <div className={styles.statBox}>
                <span className={styles.statLabel}>Completed</span>
                <span className={styles.statValue}>-</span>
              </div>
            </div>

            <button 
              onClick={() => setIsHandbookModalOpen(true)}
              className={styles.handbookButtonLarge}
            >
              📘 View Student Handbook
            </button>
          </div>
        );
      case 'exams':
        return (
          <AssignmentAccess 
            onLoginRequest={handleLoginRequest} 
            onAssignmentOpen={handleAssignmentOpen}
            currentUser={currentUser}
            onUserUpdate={refreshUserInfo}
          />
        );
      case 'results':
        return (
          <div className={styles.tabContent}>
            <h3>My Exam Results</h3>
            <div className={styles.emptyResults}>
              <img src="https://cdn-icons-png.flaticon.com/512/7486/7486744.png" alt="" style={{width: '64px', opacity: 0.5}} />
              <p>Your official results and report cards will appear here once published.</p>
            </div>
          </div>
        );
      case 'profile':
        return (
          <div className={styles.tabContent}>
            <h3>My Profile</h3>
            <div className={styles.profileCard}>
              <div className={styles.profileInfo}>
                <div className={styles.infoRow}>
                  <label>Full Name</label>
                  <span>{currentUser?.fullName || 'Not provided'}</span>
                </div>
                <div className={styles.infoRow}>
                  <label>Admission Number</label>
                  <span>{currentUser?.admissionNumber || 'Not provided'}</span>
                </div>
                {currentUser?.email && (
                  <div className={styles.infoRow}>
                    <label>Email</label>
                    <span>{currentUser.email}</span>
                  </div>
                )}
              </div>
              <button className={styles.logoutButton} onClick={() => navigate(ROUTES.LOGIN)}>
                Sign Out
              </button>
            </div>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.content}>
        <AnimatePresence mode="wait">
          {isHandbookModalOpen && (
            <HandbookModal 
              url="/handbooks/student_handbook.md" 
              title="Student Handbook"
              onClose={() => setIsHandbookModalOpen(false)} 
            />
          )}
        </AnimatePresence>

        {renderContent()}
      </div>

      {/* Mobile Bottom Navigation */}
      <nav className={styles.bottomNav}>
        <button 
          className={`${styles.bottomNavItem} ${activeTab === 'overview' ? styles.active : ''}`}
          onClick={() => setActiveTab('overview')}
        >
          <img src="https://cdn-icons-png.flaticon.com/512/1946/1946436.png" alt="Home" className={styles.bottomNavIcon} />
          <span className={styles.bottomNavLabel}>Overview</span>
        </button>

        <button 
          className={`${styles.bottomNavItem} ${activeTab === 'exams' ? styles.active : ''}`}
          onClick={() => setActiveTab('exams')}
        >
          <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="Exams" className={styles.bottomNavIcon} />
          <span className={styles.bottomNavLabel}>Assessments</span>
        </button>
        
        <button 
          className={`${styles.bottomNavItem} ${activeTab === 'results' ? styles.active : ''}`}
          onClick={() => setActiveTab('results')}
        >
          <img src="https://cdn-icons-png.flaticon.com/512/2641/2641409.png" alt="Results" className={styles.bottomNavIcon} />
          <span className={styles.bottomNavLabel}>Results</span>
        </button>

        <button 
          className={`${styles.bottomNavItem} ${activeTab === 'profile' ? styles.active : ''}`}
          onClick={() => setActiveTab('profile')}
        >
          <img src="https://cdn-icons-png.flaticon.com/512/3135/3135715.png" alt="Profile" className={styles.bottomNavIcon} />
          <span className={styles.bottomNavLabel}>Profile</span>
        </button>
      </nav>
    </div>
  );
};


export default MyAssignments;
