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
    switch (status) {
      case 'active':
        return { text: 'Active Now', className: styles.activeBadge };
      case 'scheduled':
        return { text: 'Scheduled', className: styles.scheduledBadge };
      case 'ended':
        return { text: 'Exam Ended', className: styles.endedBadge };
      default:
        return { text: 'Unknown', className: styles.endedBadge };
    }
  };

  return (
    <div className={styles.container}>
      {mode === 'guest' && !examTakerInfo ? (
        <div className={styles.guestAccess}>
          <h2 className={styles.title}>Access Your Subject Exams</h2>
          <p className={styles.introText}>
            Select your preferred method to access your exams today.
          </p>
          
          <div className={styles.accessOptions}>
            {/* Exam Taker Access */}
            <div className={styles.examTakerSection}>
              <div className={styles.optionHeader}>
                <img src="https://cdn-icons-png.flaticon.com/512/3126/3126647.png" alt="" style={{width: '32px', height: '32px'}} />
                <h3 className={styles.sectionTitle}>Quick Access with Student ID</h3>
              </div>
              <p className={styles.sectionDescription}>
                <strong>For students:</strong> Enter your unique student ID to quickly access your exams without needing a username or password.
              </p>
              
              <div className={styles.inputGroup}>
                <input
                  type="text"
                  placeholder="Enter your student ID"
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
                  {loading ? 'Checking...' : 'Access My Exams'}
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
                  <h3 className={styles.sectionTitle}>Full Account Access</h3>
                </div>
                <p className={styles.sectionDescription}>
                  <strong>Have username and password?</strong> Log in to your account for full access to exams, progress tracking, and additional features.
                </p>
                <button 
                  onClick={onLoginRequest}
                  className={styles.loginButton}
                >
                  Login to Your Account
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
                Welcome back{`${examTakerInfo.fullName ? `, ${examTakerInfo.fullName}` : ''}`}
              </h2>
              {examTakerInfo.email && (
                <p className={styles.examTakerEmail}>{examTakerInfo.email}</p>
              )}
            </div>
            {mode === 'guest' && (
              <button 
                onClick={clearExamTakerData}
                className={styles.switchUserButton}
              >
                Switch User
              </button>
            )}
          </div>
        </div>
      ) : mode === 'authenticated' ? (
        <div className={styles.authenticatedAccess}>
          <h2 className={styles.title}>
            Your Exam Hall
          </h2>
        </div>
      ) : null}

      {/* Assignment List Header */}
      {(mode === 'authenticated' || examTakerInfo) && (assignments.length > 0 || loading) && (
        <>
          {!loading && (
            <div className={styles.demoInfoBox}>
              <div className={styles.demoInfoHeader}>
                <img src="https://cdn-icons-png.flaticon.com/512/1067/1067555.png" alt="" className={styles.demoInfoIcon} />
                <h3 className={styles.demoInfoTitle}>New to the platform?</h3>
              </div>
              <p className={styles.demoInfoText}>
                Before launching a module, you can use <strong>Demo Mode</strong> to familiarize yourself with the exam experience. 
                Try it out to understand how questions work, navigation, and submission process.
              </p>
              <a href="/demo" className={styles.demoInfoLink}>
                Try Demo Mode →
              </a>
            </div>
          )}
          <h3 className={styles.assignmentsHeader}>Your Available Exams</h3>
        </>
      )}

      {/* Assignment List */}
      {(mode === 'authenticated' || (mode === 'guest' && examTakerInfo)) && (
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
          ) : assignments.length === 0 ? (
            <div className={styles.emptyContainer}>
              <p className={styles.emptyMessage}>No exams available at this time.</p>
            </div>
          ) : (
            <div className={styles.cardsGrid}>
              {assignments.map(assignment => {
                const statusBadge = getStatusBadge(assignment);
                const status = getAssignmentStatus(assignment);
                const isAccessible = assignment.isPublished && (status === 'active' || status === 'ended');
                
                return (
                  <div key={assignment.id} className={styles.assignmentCard} onClick={() => isAccessible && onAssignmentOpen && onAssignmentOpen(assignment)}>
                    <div className={styles.assignmentHeader}>
                      <h4 className={styles.assignmentTitle}>{assignment.title}</h4>
                      <div className={styles.badgeContainer}>
                        {!assignment.isPublished && (
                          <span className={styles.draftBadge}>Draft</span>
                        )}
                        {assignment.isPublished && (
                          <span className={statusBadge.className}>{statusBadge.text}</span>
                        )}
                      </div>
                    </div>
                    <p 
                      className={styles.assignmentDescription}
                      title={assignment.description || 'No description available'}
                    >
                      {assignment.description 
                        ? assignment.description.length > 120 
                          ? `${assignment.description.substring(0, 120)}...` 
                          : assignment.description
                        : 'No description available'
                      }
                    </p>
                    <div className={styles.assignmentMeta}>
                      <div className={styles.dateInfo}>
                        <span className={styles.metaLabel}>Start:</span>
                        <span>{formatDateToLocal(assignment.startDateUtc)}</span>
                      </div>
                    </div>
                    {isAccessible && (
                      <button 
                        className={styles.startButton}
                        onClick={(e) => {
                           e.stopPropagation();
                           onAssignmentOpen && onAssignmentOpen(assignment);
                        }}
                      >
                        {status === 'ended' ? 'View Results' : 'Open Exam'}
                      </button>
                    )}
                    {assignment.isPublished && status === 'scheduled' && (
                      <button className={styles.disabledButton} disabled>
                        Not Yet Available
                      </button>
                    )}
                  </div>
                );
              })}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default AssignmentAccess;
