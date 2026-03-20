import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AssignmentAccess from '../components/AssignmentAccess/AssignmentAccess';
import { Assignment } from '../models/assignment';
import { getUserInformation, type CurrentUser } from '../utils/tokenUtils';
import { CONSTANTS, ROUTES } from '../constants/contstants';
import { StudentState } from '../models/student-state';
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
      return;
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
    <div style={styles.container}>
      <div style={styles.content}>
        {/* Student Handbook Link */}
        <div style={{ marginBottom: '1.5rem', textAlign: 'right' }}>
          <button 
            onClick={() => setIsHandbookModalOpen(true)}
            style={{ 
              display: 'inline-flex', 
              alignItems: 'center', 
              gap: '8px', 
              color: '#4f46e5', 
              fontWeight: 600, 
              border: 'none',
              cursor: 'pointer',
              fontSize: '0.9rem',
              padding: '8px 16px',
              backgroundColor: 'white',
              borderRadius: '10px',
              boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)'
            }}
          >
            <span>📘 View Student Handbook</span>
          </button>
        </div>
        
        {isHandbookModalOpen && (
          <HandbookModal 
            url="/handbooks/student_handbook.md" 
            title="Student Handbook"
            onClose={() => setIsHandbookModalOpen(false)} 
          />
        )}
        <AssignmentAccess 
          onLoginRequest={handleLoginRequest} 
          onAssignmentOpen={handleAssignmentOpen}
          currentUser={currentUser}
          onUserUpdate={refreshUserInfo}
        />
      </div>
    </div>
  );
};

const styles: Record<string, React.CSSProperties> = {
  container: {
    minHeight: 'calc(100vh - 80px)',
    background: 'linear-gradient(135deg, #f9fafb 0%, #e5e7eb 100%)',
    padding: '2rem 1rem',
  },
  content: {
    maxWidth: '1200px',
    margin: '0 auto',
  },
};

export default MyAssignments;
