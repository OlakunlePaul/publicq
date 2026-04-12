import React, { useState, useEffect } from 'react';
import styles from './AssignmentAccess.module.css';
import { Assignment } from '../../models/assignment';
import { User } from '../../models/user';
import { assignmentService } from '../../services/assignmentService';
import { userService } from '../../services/userService';
import { formatDateToLocal } from '../../utils/dateUtils';

interface AssignmentAccessProps {
  onAssignmentOpen?: (assignment: Assignment) => void;
  onLoginRequest?: () => void;
  currentUser?: any; // To match AssignmentDashboard usage
  onUserUpdate?: () => void; // To match AssignmentDashboard usage
}

const AssignmentAccess: React.FC<AssignmentAccessProps> = ({ 
  onAssignmentOpen,
  onLoginRequest,
  currentUser,
  onUserUpdate
}) => {
  const [examTakerId, setExamTakerId] = useState('');
  const [examTakerInfo, setExamTakerInfo] = useState<User | null>(null);
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [userIdError, setUserIdError] = useState('');

  const [activeTab, setActiveTab] = useState<'hall' | 'archive'>('hall');

  // Determine mode based on props
  const mode = currentUser ? 'authenticated' : 'guest';

  // Load guest data if exists
  useEffect(() => {
    if (mode === 'guest') {
      const savedInfo = localStorage.getItem('exam_taker_info');
      if (savedInfo) {
        const parsed = JSON.parse(savedInfo);
        setExamTakerInfo(parsed);
        fetchAssignments(parsed.id);
      }
    } else if (currentUser) {
      setExamTakerInfo(currentUser);
      fetchAssignments(currentUser.id);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [mode, currentUser]);

  const fetchAssignments = async (userId?: string) => {
    if (!userId && mode === 'authenticated') return; // Wait for user
    
    setLoading(true);
    setError('');
    try {
      // Use the actual service method name
      const response = await assignmentService.getAvailableAssignments(userId || '');
      if (response.isSuccess && response.data) {
        setAssignments(response.data);
      } else {
        setError(response.message || 'Failed to load exams.');
      }
    } catch (err) {
      setError('An error occurred while fetching exams.');
    } finally {
      setLoading(false);
    }
  };

  const handleGuestAccess = async () => {
    if (!examTakerId.trim()) {
      setUserIdError('Please enter an ID');
      return;
    }

    setLoading(true);
    setUserIdError('');
    try {
      // Use userService instead of assignmentService for exam taker info
      const response = await userService.getStudent(examTakerId);
      if (response.isSuccess && response.data) {
        setExamTakerInfo(response.data);
        localStorage.setItem('exam_taker_info', JSON.stringify(response.data));
        fetchAssignments(response.data.id);
        if (onUserUpdate) onUserUpdate();
      } else {
        setUserIdError('Invalid Exam Taker ID. Please check and try again.');
      }
    } catch (err) {
      setUserIdError('Failed to verify ID. Please try again later.');
    } finally {
      setLoading(false);
    }
  };

  const clearExamTakerData = () => {
    localStorage.removeItem('exam_taker_info');
    setExamTakerInfo(null);
    setExamTakerId('');
    setAssignments([]);
    if (onUserUpdate) onUserUpdate();
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleGuestAccess();
    }
  };

  const getAssignmentStatus = (assignment: Assignment) => {
    const now = new Date();
    const start = new Date(assignment.startDateUtc);
    const end = new Date(assignment.endDateUtc);

    if (now < start) return 'scheduled';
    if (now > end) return 'ended';
    return 'active';
  };

  const getStatusBadge = (assignment: Assignment) => {
    const status = getAssignmentStatus(assignment);
    const isCompleted = assignment.isCompleted;

    if (isCompleted) return { text: 'Completed', className: `${styles.badge} ${styles.completedBadge}` };
    
    switch (status) {
      case 'active':
        return { text: 'Active Now', className: `${styles.badge} ${styles.activeBadge}` };
      case 'scheduled':
        return { text: 'Scheduled', className: `${styles.badge} ${styles.scheduledBadge}` };
      case 'ended':
        return { text: 'Exam Ended', className: `${styles.badge} ${styles.endedBadge}` };
      default:
        return { text: 'Unknown', className: `${styles.badge} ${styles.endedBadge}` };
    }
  };

  // Filter assignments
  const activeAssignments = assignments.filter(a => !a.isCompleted && getAssignmentStatus(a) !== 'ended');
  const archivedAssignments = assignments.filter(a => a.isCompleted || getAssignmentStatus(a) === 'ended');
  const displayAssignments = activeTab === 'hall' ? activeAssignments : archivedAssignments;

  return (
    <div className={styles.container}>
      {mode === 'guest' && !examTakerInfo ? (
        <div className={styles.guestAccess}>
          <h2 className={styles.title}>Access Your Exams</h2>
          <p className={styles.introText}>
            Enter your student ID to enter the exam hall and view your scheduled papers.
          </p>
          
          <div className={styles.accessOptions}>
            {/* Exam Taker Access */}
            <div className={styles.examTakerSection}>
              <div className={styles.optionHeader}>
                <img src="https://cdn-icons-png.flaticon.com/512/3126/3126647.png" alt="" style={{width: '32px', height: '32px'}} />
                <h3 className={styles.sectionTitle}>Quick Entry</h3>
              </div>
              <p className={styles.sectionDescription}>
                Enter your unique student ID to quickly access your assigned sessions.
              </p>
              
              <div className={styles.inputGroup}>
                <input
                  type="text"
                  placeholder="Student ID (e.g. PQ-2024-001)"
                  value={examTakerId}
                  onChange={(e) => {
                    setExamTakerId(e.target.value.toUpperCase());
                    setUserIdError('');
                  }}
                  onKeyPress={handleKeyPress}
                  className={userIdError ? styles.inputError : styles.input}
                />
                <button 
                  onClick={handleGuestAccess}
                  className={styles.accessButton}
                  disabled={loading}
                >
                  {loading ? 'Verifying...' : 'Enter Exam Hall'}
                </button>
              </div>
              
              {userIdError && (
                <p className={styles.errorText}>{userIdError}</p>
              )}
            </div>

            {/* Login Option */}
            {onLoginRequest && (
              <div className={styles.loginSection}>
                <div className={styles.optionHeader}>
                  <img src="https://cdn-icons-png.flaticon.com/512/3064/3064155.png" alt="" style={{width: '32px', height: '32px'}} />
                  <h3 className={styles.sectionTitle}>Login</h3>
                </div>
                <p className={styles.sectionDescription}>
                  Have an account password? Log in for unified access to all features.
                </p>
                <button 
                  onClick={onLoginRequest}
                  className={styles.loginButton}
                >
                  Log in to Account
                </button>
              </div>
            )}
          </div>
        </div>
      ) : examTakerInfo ? (
        <div className={styles.examTakerWelcome}>
          <div className={styles.welcomeHeader}>
            <img src="https://cdn-icons-png.flaticon.com/512/9371/9371842.png" alt="" style={{width: '48px', height: '48px'}} />
            <div className={styles.welcomeContent}>
              <h2 className={styles.welcomeTitle}>
                Hi, {examTakerInfo.fullName || 'Student'}
              </h2>
              <p className={styles.examTakerEmail}>
                {examTakerInfo.admissionNumber || examTakerInfo.id || 'Ready for assignments?'}
              </p>
            </div>
            {mode === 'guest' && (
              <button 
                onClick={clearExamTakerData}
                className={styles.switchUserButton}
              >
                Sign Out
              </button>
            )}
          </div>
        </div>
      ) : mode === 'authenticated' ? (
        <div className={styles.authenticatedAccess}>
          <h2 className={styles.title}>
            Exam Hall
          </h2>
        </div>
      ) : null}

      {/* Main Content Area */}
      {(mode === 'authenticated' || (mode === 'guest' && examTakerInfo)) && (
        <>
          {/* Tab Navigation */}
          <div className={styles.tabNav}>
            <button 
              className={`${styles.tabButton} ${activeTab === 'hall' ? styles.activeTab : ''}`}
              onClick={() => setActiveTab('hall')}
            >
              Active Hall
              <span className={styles.tabCount}>{activeAssignments.length}</span>
            </button>
            <button 
              className={`${styles.tabButton} ${activeTab === 'archive' ? styles.activeTab : ''}`}
              onClick={() => setActiveTab('archive')}
            >
              Archive
              <span className={styles.tabCount}>{archivedAssignments.length}</span>
            </button>
          </div>

          <div className={styles.assignmentList}>
            {loading ? (
              <div className={styles.skeletonGrid}>
                {[1, 2, 3].map(i => (
                  <div key={i} className={styles.skeletonCard}></div>
                ))}
              </div>
            ) : error ? (
              <div className={styles.errorContainer}>
                <p className={styles.errorText}>{error}</p>
              </div>
            ) : displayAssignments.length === 0 ? (
              <div className={styles.emptyContainer}>
                <div className={styles.emptyIcon}>📭</div>
                <p className={styles.emptyMessage}>
                  {activeTab === 'hall' 
                    ? 'No active exams scheduled right now.' 
                    : 'Your exam history is currently empty.'}
                </p>
              </div>
            ) : (
              <div className={styles.cardsGrid}>
                {displayAssignments.map(assignment => {
                  const statusBadge = getStatusBadge(assignment);
                  const status = getAssignmentStatus(assignment);
                  const isLocked = assignment.isProgressionLocked;
                  const isCompleted = assignment.isCompleted;
                  const isAccessible = assignment.isPublished && (status === 'active' || status === 'ended') && !isLocked;
                  
                  return (
                    <div 
                      key={assignment.id} 
                      className={`${styles.assignmentCard} ${isLocked ? styles.lockedCard : ''}`} 
                      onClick={() => isAccessible && onAssignmentOpen && onAssignmentOpen(assignment)}
                    >
                      <div className={styles.assignmentHeader}>
                        <h4 className={styles.assignmentTitle}>{assignment.title}</h4>
                        <div className={styles.badgeContainer}>
                          {isLocked ? (
                            <span className={`${styles.badge} ${styles.lockedBadge}`}>
                              <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3"><rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect><path d="M7 11V7a5 5 0 0 1 10 0v4"></path></svg>
                              Locked
                            </span>
                          ) : (
                            <span className={statusBadge.className}>{statusBadge.text}</span>
                          )}
                        </div>
                      </div>
                      
                      <p className={styles.assignmentDescription}>
                        {assignment.description || 'Access your exam module and follow the instructions carefully.'}
                      </p>
                      
                      <div className={styles.assignmentMeta}>
                        <div className={styles.dateInfo}>
                          <span className={styles.metaLabel}>Scheduled:</span>
                          <span>{formatDateToLocal(assignment.startDateUtc)}</span>
                        </div>
                      </div>
                      
                      {isLocked ? (
                        <button className={styles.disabledButton} disabled>
                           Complete Previous First
                        </button>
                      ) : isAccessible ? (
                        <button 
                          className={styles.startButton}
                          onClick={(e) => {
                             e.stopPropagation();
                             onAssignmentOpen && onAssignmentOpen(assignment);
                          }}
                        >
                          {status === 'ended' || isCompleted ? 'View Results' : 'Launch Exam'}
                        </button>
                      ) : (assignment.isPublished && status === 'scheduled') ? (
                        <button className={styles.disabledButton} disabled>
                          Available Soon
                        </button>
                      ) : null}
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
};

export default AssignmentAccess;

