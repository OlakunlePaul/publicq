import React, { useState, useEffect } from 'react';
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
        setError(response.message || 'Failed to load assignments.');
      }
    } catch (err) {
      setError('An error occurred while fetching assignments.');
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
      const response = await userService.getExamTaker(examTakerId);
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
        return { text: 'Active Now', style: styles.activeBadge };
      case 'scheduled':
        return { text: 'Scheduled', style: styles.scheduledBadge };
      case 'ended':
        return { text: 'Exam Ended', style: styles.endedBadge };
      default:
        return { text: 'Unknown', style: styles.endedBadge };
    }
  };

  return (
    <div style={styles.container}>
      {mode === 'guest' && !examTakerInfo ? (
        <div style={styles.guestAccess}>
          <h2 style={styles.title}>Access Your Subject Exams</h2>
          <p style={styles.introText}>
            Select your preferred method to access your assignments today.
          </p>
          
          <div style={styles.accessOptions}>
            {/* Exam Taker Access */}
            <div style={styles.examTakerSection}>
              <div style={styles.optionHeader}>
                <img src="https://cdn-icons-png.flaticon.com/512/3126/3126647.png" alt="" style={{width: '32px', height: '32px'}} />
                <h3 style={styles.sectionTitle}>Quick Access with Exam Taker ID</h3>
              </div>
              <p style={styles.sectionDescription}>
                <strong>For exam takers:</strong> Enter your unique exam taker ID to quickly access your assignments without needing a username or password.
              </p>
              
              <div style={styles.inputGroup}>
                <input
                  type="text"
                  placeholder="Enter your exam taker ID"
                  value={examTakerId}
                  onChange={(e) => {
                    setExamTakerId(e.target.value.toUpperCase());
                    setUserIdError('');
                  }}
                  onKeyPress={handleKeyPress}
                  style={userIdError ? styles.inputError : styles.input}
                  onFocus={(e) => {
                    e.currentTarget.style.borderColor = '#4f46e5';
                    e.currentTarget.style.boxShadow = '0 0 0 4px rgba(79, 70, 229, 0.1)';
                  }}
                  onBlur={(e) => {
                    e.currentTarget.style.borderColor = userIdError ? '#dc2626' : '#e2e8f0';
                    e.currentTarget.style.boxShadow = 'none';
                  }}
                />
                <button 
                  onClick={handleGuestAccess}
                  style={styles.accessButton}
                  disabled={loading}
                >
                  {loading ? 'Checking...' : 'Access My Exams'}
                </button>
              </div>
              
              {userIdError && (
                <p style={styles.errorText}>{userIdError}</p>
              )}
            </div>

            {/* Login Option */}
            {onLoginRequest && (
              <div style={styles.loginSection}>
                <div style={styles.optionHeader}>
                  <img src="https://cdn-icons-png.flaticon.com/512/3064/3064155.png" alt="" style={{width: '32px', height: '32px'}} />
                  <h3 style={styles.sectionTitle}>Full Account Access</h3>
                </div>
                <p style={styles.sectionDescription}>
                  <strong>Have username and password?</strong> Log in to your account for full access to assignments, progress tracking, and additional features.
                </p>
                <button 
                  onClick={onLoginRequest}
                  style={styles.loginButton}
                >
                  Login to Your Account
                </button>
              </div>
            )}
          </div>
        </div>
      ) : examTakerInfo ? (
        <div style={styles.examTakerWelcome}>
          <div style={styles.welcomeHeader}>
            <img src="https://cdn-icons-png.flaticon.com/512/9371/9371842.png" alt="" style={{width: '48px', height: '48px'}} />
            <div style={styles.welcomeContent}>
              <h2 style={styles.welcomeTitle}>
                Welcome back{`${examTakerInfo.fullName ? `, ${examTakerInfo.fullName}` : ''}`}
              </h2>
              {examTakerInfo.email && (
                <p style={styles.examTakerEmail}>{examTakerInfo.email}</p>
              )}
            </div>
            {mode === 'guest' && (
              <button 
                onClick={clearExamTakerData}
                style={styles.switchUserButton}
              >
                Switch User
              </button>
            )}
          </div>
        </div>
      ) : mode === 'authenticated' ? (
        <div style={styles.authenticatedAccess}>
          <h2 style={styles.title}>
            Your Assignment Dashboard
          </h2>
        </div>
      ) : null}

      {/* Assignment List Header */}
      {(mode === 'authenticated' || examTakerInfo) && assignments.length > 0 && (
        <>
          <div style={styles.demoInfoBox}>
            <div style={styles.demoInfoHeader}>
              <img src="https://cdn-icons-png.flaticon.com/512/1067/1067555.png" alt="" style={styles.demoInfoIcon} />
              <h3 style={styles.demoInfoTitle}>New to the platform?</h3>
            </div>
            <p style={styles.demoInfoText}>
              Before launching a module, you can use <strong>Demo Mode</strong> to familiarize yourself with the exam experience. 
              Try it out to understand how questions work, navigation, and submission process.
            </p>
            <a href="/demo" style={styles.demoInfoLink}>
              Try Demo Mode →
            </a>
          </div>
          <h3 style={styles.assignmentsHeader}>Your Available Exams</h3>
        </>
      )}

      {/* Assignment List */}
      {(mode === 'authenticated' || (mode === 'guest' && examTakerInfo)) && (
        <div style={styles.assignmentList}>
          {loading ? (
            <div style={styles.loadingContainer}>
              <p style={styles.loadingText}>Loading assignments...</p>
            </div>
          ) : error ? (
            <div style={styles.errorContainer}>
              <p style={styles.errorText}>{error}</p>
            </div>
          ) : assignments.length === 0 ? (
            <div style={styles.emptyContainer}>
              <p style={styles.emptyMessage}>No assignments available at this time.</p>
            </div>
          ) : (
            <div style={styles.cardsGrid}>
              {assignments.map(assignment => {
                const statusBadge = getStatusBadge(assignment);
                const status = getAssignmentStatus(assignment);
                const isAccessible = assignment.isPublished && (status === 'active' || status === 'ended');
                
                return (
                  <div key={assignment.id} style={styles.assignmentCard}>
                    <div style={styles.assignmentHeader}>
                      <h4 style={styles.assignmentTitle}>{assignment.title}</h4>
                      <div style={styles.badgeContainer}>
                        {!assignment.isPublished && (
                          <span style={styles.draftBadge}>Draft</span>
                        )}
                        {assignment.isPublished && (
                          <span style={statusBadge.style}>{statusBadge.text}</span>
                        )}
                      </div>
                    </div>
                    <p 
                      style={styles.assignmentDescription}
                      title={assignment.description || 'No description available'}
                    >
                      {assignment.description 
                        ? assignment.description.length > 120 
                          ? `${assignment.description.substring(0, 120)}...` 
                          : assignment.description
                        : 'No description available'
                      }
                    </p>
                    <div style={styles.assignmentMeta}>
                      <div style={styles.dateInfo}>
                        <span style={styles.metaLabel}>Start:</span>
                        <span>{formatDateToLocal(assignment.startDateUtc)}</span>
                      </div>
                    </div>
                    {isAccessible && (
                      <button 
                        style={styles.startButton}
                        onClick={() => onAssignmentOpen && onAssignmentOpen(assignment)}
                      >
                        {status === 'ended' ? 'View Results' : 'Open Assignment'}
                      </button>
                    )}
                    {assignment.isPublished && status === 'scheduled' && (
                      <button style={styles.disabledButton} disabled>
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

const styles: Record<string, React.CSSProperties> = {
  container: {
    maxWidth: '1000px',
    margin: '0 auto',
    padding: '40px 20px',
    fontFamily: "'Inter', system-ui, sans-serif",
  },
  title: {
    fontSize: '32px',
    fontWeight: '800',
    marginBottom: '16px',
    textAlign: 'center',
    color: '#1e293b',
    letterSpacing: '-0.025em',
  },
  introText: {
    fontSize: '16px',
    color: '#64748b',
    textAlign: 'center',
    marginBottom: '40px',
    lineHeight: '1.6',
  },
  assignmentsHeader: {
    fontSize: '24px',
    fontWeight: '800',
    marginBottom: '24px',
    marginTop: '40px',
    color: '#1e293b',
  },
  demoInfoBox: {
    padding: '24px',
    backgroundColor: '#eff6ff',
    border: '1px solid #dbeafe',
    borderRadius: '16px',
    marginBottom: '32px',
  },
  demoInfoHeader: {
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
    marginBottom: '12px',
  },
  demoInfoIcon: {
    width: '28px',
    height: '28px',
  },
  demoInfoTitle: {
    fontSize: '18px',
    fontWeight: '700',
    margin: '0',
    color: '#1e40af',
  },
  demoInfoText: {
    fontSize: '15px',
    color: '#1e40af',
    lineHeight: '1.6',
    marginBottom: '16px',
  },
  demoInfoLink: {
    display: 'inline-flex',
    alignItems: 'center',
    fontSize: '15px',
    fontWeight: '700',
    color: '#2563eb',
    textDecoration: 'none',
  },
  guestAccess: {
    marginBottom: '40px',
  },
  accessOptions: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
    gap: '24px',
  },
  optionHeader: {
    display: 'flex',
    alignItems: 'center',
    gap: '16px',
    marginBottom: '16px',
  },
  examTakerSection: {
    padding: '32px',
    border: '1px solid #e2e8f0',
    borderRadius: '20px',
    backgroundColor: 'white',
    boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.04)',
  },
  loginSection: {
    padding: '32px',
    border: '1px solid #e2e8f0',
    borderRadius: '20px',
    backgroundColor: 'white',
    boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.04)',
  },
  sectionTitle: {
    fontSize: '20px',
    fontWeight: '700',
    margin: '0',
  },
  sectionDescription: {
    fontSize: '15px',
    color: '#64748b',
    marginBottom: '24px',
    lineHeight: '1.6',
  },
  inputGroup: {
    display: 'flex',
    flexDirection: 'column',
    gap: '12px',
  },
  input: {
    padding: '14px 16px',
    border: '1px solid #e2e8f0',
    borderRadius: '12px',
    fontSize: '15px',
    backgroundColor: '#f8fafc',
  },
  inputError: {
    padding: '14px 16px',
    border: '1px solid #dc2626',
    borderRadius: '12px',
    fontSize: '15px',
    backgroundColor: '#fef2f2',
  },
  accessButton: {
    padding: '14px 24px',
    backgroundColor: '#4f46e5',
    color: 'white',
    border: 'none',
    borderRadius: '12px',
    cursor: 'pointer',
    fontSize: '15px',
    fontWeight: '700',
  },
  loginButton: {
    width: '100%',
    padding: '14px 24px',
    backgroundColor: '#10b981',
    color: 'white',
    border: 'none',
    borderRadius: '12px',
    cursor: 'pointer',
    fontSize: '15px',
    fontWeight: '700',
  },
  examTakerWelcome: {
    marginBottom: '40px',
    padding: '32px',
    backgroundColor: 'white',
    borderRadius: '24px',
    border: '1px solid #e2e8f0',
  },
  welcomeHeader: {
    display: 'flex',
    alignItems: 'center',
    gap: '20px',
  },
  welcomeContent: {
    flex: 1,
  },
  welcomeTitle: {
    fontSize: '26px',
    fontWeight: '800',
    margin: '0',
  },
  examTakerEmail: {
    fontSize: '15px',
    color: '#64748b',
    margin: '0',
  },
  switchUserButton: {
    padding: '10px 20px',
    backgroundColor: 'white',
    color: '#475569',
    border: '1px solid #e2e8f0',
    borderRadius: '10px',
    cursor: 'pointer',
    fontSize: '14px',
    fontWeight: '600',
  },
  cardsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
    gap: '24px',
  },
  assignmentCard: {
    padding: '28px',
    border: '1px solid #e2e8f0',
    borderRadius: '24px',
    backgroundColor: 'white',
    display: 'flex',
    flexDirection: 'column',
    height: '100%',
  },
  assignmentHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: '16px',
  },
  assignmentTitle: {
    fontSize: '20px',
    fontWeight: '800',
    margin: '0',
    lineHeight: '1.3',
  },
  badgeContainer: {
    marginLeft: '12px',
  },
  assignmentDescription: {
    fontSize: '14px',
    color: '#64748b',
    marginBottom: '24px',
    lineHeight: '1.6',
    flex: 1,
  },
  assignmentMeta: {
    marginBottom: '24px',
    paddingTop: '16px',
    borderTop: '1px solid #f1f5f9',
  },
  dateInfo: {
    display: 'flex',
    alignItems: 'center',
    gap: '8px',
    fontSize: '13px',
    color: '#64748b',
  },
  metaLabel: {
    fontWeight: '700',
    color: '#475569',
    textTransform: 'uppercase',
    fontSize: '11px',
    letterSpacing: '0.05em',
  },
  startButton: {
    padding: '14px',
    backgroundColor: '#4f46e5',
    color: 'white',
    border: 'none',
    borderRadius: '14px',
    cursor: 'pointer',
    fontSize: '15px',
    fontWeight: '700',
    textAlign: 'center',
  },
  disabledButton: {
    padding: '14px',
    backgroundColor: '#e2e8f0',
    color: '#94a3b8',
    border: 'none',
    borderRadius: '14px',
    cursor: 'not-allowed',
    fontSize: '15px',
    fontWeight: '700',
    textAlign: 'center',
  },
  activeBadge: {
    padding: '4px 12px',
    backgroundColor: '#d1fae5',
    color: '#065f46',
    borderRadius: '20px',
    fontSize: '12px',
    fontWeight: '700',
  },
  scheduledBadge: {
    padding: '4px 12px',
    backgroundColor: '#fef3c7',
    color: '#92400e',
    borderRadius: '20px',
    fontSize: '12px',
    fontWeight: '700',
  },
  endedBadge: {
    padding: '4px 12px',
    backgroundColor: '#f3f4f6',
    color: '#374151',
    borderRadius: '20px',
    fontSize: '12px',
    fontWeight: '700',
  },
  draftBadge: {
    padding: '4px 12px',
    backgroundColor: '#fef2f2',
    color: '#991b1b',
    borderRadius: '20px',
    fontSize: '12px',
    fontWeight: '700',
  },
  loadingContainer: {
    textAlign: 'center',
    padding: '40px',
  },
  loadingText: {
    color: '#4f46e5',
    fontSize: '16px',
    fontWeight: '600',
  },
  emptyContainer: {
    textAlign: 'center',
    padding: '60px 20px',
    backgroundColor: '#f8fafc',
    borderRadius: '24px',
    border: '2px dashed #e2e8f0',
  },
  emptyMessage: {
    color: '#94a3b8',
    fontSize: '16px',
    fontStyle: 'italic',
  },
  errorContainer: {
    padding: '16px',
    backgroundColor: '#fef2f2',
    borderRadius: '12px',
    border: '1px solid #fee2e2',
    marginBottom: '24px',
  },
  errorText: {
    color: '#dc2626',
    fontSize: '14px',
    fontWeight: '500',
    margin: '0',
  },
};

export default AssignmentAccess;
