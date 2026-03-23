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

  return (
    <div className={styles.container}>
      <div className={styles.content}>
        {/* Student Handbook Link */}
        <div style={{ marginBottom: '1.5rem', textAlign: 'right' }}>
          <button 
            onClick={() => setIsHandbookModalOpen(true)}
            className={styles.handbookButton}
          >
            <span>📘 View Student Handbook</span>
          </button>
        </div>
        
        <AnimatePresence mode="wait">
          {isHandbookModalOpen && (
            <HandbookModal 
              url="/handbooks/student_handbook.md" 
              title="Student Handbook"
              onClose={() => setIsHandbookModalOpen(false)} 
            />
          )}
        </AnimatePresence>

        <AssignmentAccess 
          onLoginRequest={handleLoginRequest} 
          onAssignmentOpen={handleAssignmentOpen}
          currentUser={currentUser}
          onUserUpdate={refreshUserInfo}
        />
      </div>

      {/* Mobile Bottom Navigation */}
      <nav className={styles.bottomNav}>
        <button 
          className={`${styles.bottomNavItem} ${styles.active}`}
          onClick={() => navigate(ROUTES.MY_EXAMS)}
        >
          <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="Exams" className={styles.bottomNavIcon} />
          <span className={styles.bottomNavLabel}>My Exams</span>
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
          onClick={() => setIsHandbookModalOpen(true)}
        >
          <img src="https://cdn-icons-png.flaticon.com/512/3306/3306631.png" alt="Handbook" className={styles.bottomNavIcon} />
          <span className={styles.bottomNavLabel}>Handbook</span>
        </button>
      </nav>
    </div>
  );
};


export default MyAssignments;
