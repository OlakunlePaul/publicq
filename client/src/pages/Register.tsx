import { useState, useEffect, useCallback, useRef } from "react";
import { useNavigate } from 'react-router-dom';
import { CONSTANTS } from '../constants/contstants';
import { UserCreateRequest } from "../models/userCreateRequest";
import { ResponseWithData } from "../models/responseWithData";
import { useAuth } from "../context/AuthContext";
import { userService } from "../services/userService";
import { configurationService } from "../services/configurationService";
import { PasswordPolicyOptions } from "../models/password-policy-options";
import { academicStructureService } from "../services/academicStructureService";
import { ClassLevelDto } from "../models/academic";
import { cn } from '../utils/cn';
import PasswordInput from '../components/Shared/PasswordInput';
import registerStyles from './Register.module.css';

const Register = () => {
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [fullName, setFullName] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [classLevels, setClassLevels] = useState<ClassLevelDto[]>([]);
  const [selectedClassLevelId, setSelectedClassLevelId] = useState('');
  const [classLoading, setClassLoading] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);
  const [touched, setTouched] = useState<{ [key: string]: boolean }>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [passwordPolicy, setPasswordPolicy] = useState<PasswordPolicyOptions | null>(null);
  const [policyLoading, setPolicyLoading] = useState(false);
  const [showPasswordInfo, setShowPasswordInfo] = useState(false);
  const [registrationEnabled, setRegistrationEnabled] = useState<boolean | null>(null);
  const [registrationLoading, setRegistrationLoading] = useState(true);
  const [isStudent, setIsStudent] = useState(true);
  const [registrationSuccessData, setRegistrationSuccessData] = useState<{ id: string, name: string } | null>(null);
  const { saveToken } = useAuth()
  const emailInputRef = useRef<HTMLInputElement>(null);

  // Auto-focus on email input when component mounts and check registration status
  useEffect(() => {
    if (emailInputRef.current) {
      emailInputRef.current.focus();
    }
    checkRegistrationStatus();
    loadClassLevels();
  }, []);

  // Load password policy only when registration is enabled
  useEffect(() => {
    if (registrationEnabled === true) {
      loadPasswordPolicy();
    }
  }, [registrationEnabled]);

  const checkRegistrationStatus = async () => {
    setRegistrationLoading(true);
    try {
      const response = await configurationService.getSelfServiceRegistration();
      setRegistrationEnabled(response.data);
    } catch (err) {
      setRegistrationEnabled(false); // Default to disabled if we can't check
    } finally {
      setRegistrationLoading(false);
    }
  };

  const loadPasswordPolicy = async () => {
    setPolicyLoading(true);
    try {
      const response = await configurationService.getPasswordPolicy();
      setPasswordPolicy(response.data);
    } catch (err) {
      // Set default policy if loading fails
      setPasswordPolicy({
        requiredLength: 6,
        requireDigit: false,
        requireUppercase: false,
        requireLowercase: false,
        requireNonAlphanumeric: false
      });
    } finally {
      setPolicyLoading(false);
    }
  };

  const loadClassLevels = async () => {
    setClassLoading(true);
    try {
      const response = await academicStructureService.getClassLevels();
      if (response.isSuccess && response.data) {
        setClassLevels(response.data);
      }
    } catch (err) {
      console.error('Failed to load class levels', err);
    } finally {
      setClassLoading(false);
    }
  };


  const getFieldError = useCallback((fieldName: string) => {
    if (!touched[fieldName]) return null;

    switch (fieldName) {
      case 'email':
        if (!isStudent && !username.trim()) return 'Email is required';
        if (username.trim() && !/\S+@\S+\.\S+/.test(username)) return 'Please enter a valid email address';
        break;
      case 'fullName':
        if (!fullName.trim()) return 'Full name is required';
        break;
      case 'password':
        if (!isStudent) {
          if (!password.trim()) return 'Password is required';
          if (passwordPolicy) {
            if (password.length < passwordPolicy.requiredLength) {
              return `Password must be at least ${passwordPolicy.requiredLength} characters long`;
            }
            if (passwordPolicy.requireDigit && !/\d/.test(password)) {
              return 'Password must contain at least one digit (0-9)';
            }
            if (passwordPolicy.requireUppercase && !/[A-Z]/.test(password)) {
              return 'Password must contain at least one uppercase letter (A-Z)';
            }
            if (passwordPolicy.requireLowercase && !/[a-z]/.test(password)) {
              return 'Password must contain at least one lowercase letter (a-z)';
            }
            if (passwordPolicy.requireNonAlphanumeric && !/[^A-Za-z0-9]/.test(password)) {
              return 'Password must contain at least one special character (!@#$%^&* etc.)';
            }
          } else if (password.length < 6) {
            return 'Password must be at least 6 characters long';
          }
        }
        break;
      case 'confirmPassword':
        if (!isStudent) {
          if (!confirmPassword.trim()) return 'Please confirm your password';
          if (password !== confirmPassword) return 'Passwords do not match';
        }
        break;
      case 'classLevelId':
        if (isStudent && !selectedClassLevelId) return 'Please select your class';
        break;
    }
    return null;
  }, [username, fullName, password, confirmPassword, passwordPolicy, touched, selectedClassLevelId, isStudent]);

  const validateForm = useCallback(() => {
    // Check if any field has validation errors by using the existing getFieldError function
    // This prevents duplication of validation messages
    const hasEmailError = getFieldError('email');
    const hasFullNameError = getFieldError('fullName');
    const hasPasswordError = getFieldError('password');
    const hasConfirmPasswordError = getFieldError('confirmPassword');
    const hasClassError = getFieldError('classLevelId');

    // Return true if any field has errors, false if all fields are valid
    return !!(hasEmailError || hasFullNameError || hasPasswordError || hasConfirmPasswordError || hasClassError);
  }, [getFieldError]);

  const handleFieldTouch = (fieldName: string) => {
    setTouched({ ...touched, [fieldName]: true });
  };

  const handleRegister = useCallback(async (e?: React.FormEvent) => {
    if (e) e.preventDefault();

    if (isSubmitting) return; // Prevent double submission

    // Mark all fields as touched for validation display
    setTouched({
      email: true,
      fullName: true,
      dateOfBirth: true,
      password: true,
      confirmPassword: true,
      classLevelId: true,
    });

    const hasValidationErrors = validateForm();
    if (hasValidationErrors) {
      setErrors([]); // Clear any previous API errors, field errors will show under inputs
      setIsSubmitting(false); // Reset submitting state if validation fails
      return;
    }

    setErrors([]);
    setIsSubmitting(true);

    try {
      if (isStudent) {
        const registerDataStudent: any = {
          fullName: fullName.trim(),
          ...(username.trim() && { email: username.trim() }),
          ...(dateOfBirth.trim() && { dateOfBirth: dateOfBirth.trim() }),
          classLevelId: selectedClassLevelId,
        };
        const response = await userService.createStudentPublic(registerDataStudent);
        setRegistrationSuccessData({
          id: response.data?.admissionNumber || response.data?.id || 'Unknown',
          name: response.data?.fullName || fullName.trim()
        });
      } else {
        const registerDataTeacher: UserCreateRequest = {
          email: username,
          fullName: fullName,
          password: password,
          ...(dateOfBirth && { dateOfBirth }),
        };
        const response = await userService.createUser(registerDataTeacher);
        const token = response.accessToken;

        localStorage.setItem(CONSTANTS.TOKEN_VARIABLE_NAME, token);
        saveToken(token);
        
        // Redirect to home page after successful registration
        navigate('/');
      }
    } catch (err: any) {
      const result = err.response?.data as ResponseWithData<string, any> | undefined;
      if (result?.errors) {
        setErrors(result.errors);
      } else {
        setErrors(['Registration failed. Please try again.']);
      }
    } finally {
      setIsSubmitting(false);
    }
  }, [username, fullName, dateOfBirth, password, isSubmitting, navigate, saveToken, validateForm, selectedClassLevelId, isStudent]);

  // Handle Ctrl+Enter keyboard shortcut
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
        e.preventDefault();
        handleRegister();
      }
    };

    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [handleRegister]);

  return (
    <div className={registerStyles.container}>
      <div className={registerStyles.registerCard}>
        {registrationLoading ? (
          <div className={registerStyles.header}>
            <h1 className={registerStyles.title}>Create an Account</h1>
            <p className={registerStyles.subtitle}>Join ExamNova to create and manage assessment modules</p>
          </div>
        ) : registrationEnabled === false ? (
          <div className={registerStyles.header}>
            <h2 className={registerStyles.title}>Registration Currently Disabled</h2>
            <p className={registerStyles.subtitle}>
              Students will get IDs and instructions from platform managers. 
              If you need a user account to manage site content, please contact the administrator.
            </p>
          </div>
        ) : (
          <div className={registerStyles.header}>
            <h2 className={registerStyles.title}>Join Our School</h2>
            <p className={registerStyles.subtitle}>
              {isStudent ? 'Enter your details below to register as a student' : 'Enter your details below to register as a teacher or parent'}
            </p>
          </div>
        )}

        {registrationLoading ? (
          <div className={registerStyles.loadingContainer}>
            <div className={registerStyles.loadingSpinner}></div>
            <p className={registerStyles.loadingText}>Checking registration availability...</p>
          </div>
        ) : registrationEnabled === true ? (
          registrationSuccessData ? (
            <div className={registerStyles.successContainer} style={{ padding: '2rem', textAlign: 'center', backgroundColor: '#f0fdf4', borderRadius: '0.5rem', border: '1px solid #bbf7d0', marginBottom: '1.5rem' }}>
              <div style={{ color: '#22c55e', fontSize: '3rem', marginBottom: '1rem' }}>✓</div>
              <h3 style={{ fontSize: '1.5rem', fontWeight: 600, color: '#166534', marginBottom: '0.5rem' }}>Registration Successful!</h3>
              <p style={{ color: '#15803d', marginBottom: '1.5rem' }}>
                Welcome, {registrationSuccessData.name}. Your account has been created.
              </p>
              <div style={{ backgroundColor: '#fff', padding: '1rem', borderRadius: '0.5rem', border: '1px solid #dcfce7', marginBottom: '1.5rem' }}>
                <p style={{ color: '#166534', fontSize: '0.875rem', fontWeight: 500, marginBottom: '0.25rem' }}>Your Admission Number / Login ID</p>
                <div style={{ fontSize: '1.5rem', fontWeight: 700, color: '#15803d', letterSpacing: '0.05em' }}>
                  {registrationSuccessData.id}
                </div>
              </div>
              <p style={{ fontSize: '0.875rem', color: '#166534', marginBottom: '1.5rem' }}>
                Please save this number. You will use it to log in as a student.
              </p>
              <button 
                onClick={() => navigate('/login')}
                className={registerStyles.submitButton}
                style={{ width: '100%' }}
              >
                Go to Login
              </button>
            </div>
          ) : (
          <>
            <div style={{ display: 'flex', marginBottom: '1.5rem', backgroundColor: '#f3f4f6', borderRadius: '0.5rem', padding: '0.25rem' }}>
              <button
                type="button"
                onClick={() => { setIsStudent(true); setErrors([]); setTouched({}); }}
                style={{ 
                  flex: 1, 
                  padding: '0.5rem', 
                  borderRadius: '0.375rem',
                  border: 'none',
                  backgroundColor: isStudent ? '#fff' : 'transparent',
                  color: isStudent ? '#1f2937' : '#4b5563',
                  fontWeight: isStudent ? 600 : 500,
                  boxShadow: isStudent ? '0 1px 3px rgba(0,0,0,0.1)' : 'none',
                  cursor: 'pointer',
                  transition: 'all 0.2s'
                }}
              >
                I am a Student
              </button>
              <button
                type="button"
                onClick={() => { setIsStudent(false); setErrors([]); setTouched({}); }}
                style={{ 
                  flex: 1, 
                  padding: '0.5rem', 
                  borderRadius: '0.375rem',
                  border: 'none',
                  backgroundColor: !isStudent ? '#fff' : 'transparent',
                  color: !isStudent ? '#1f2937' : '#4b5563',
                  fontWeight: !isStudent ? 600 : 500,
                  boxShadow: !isStudent ? '0 1px 3px rgba(0,0,0,0.1)' : 'none',
                  cursor: 'pointer',
                  transition: 'all 0.2s'
                }}
              >
                Teacher / Parent
              </button>
            </div>

            {errors.length > 0 && (
              <div className={registerStyles.errorContainer}>
                <ul className={registerStyles.errorList}>
                  {errors.map((error, index) => (
                    <li key={index} className={registerStyles.errorItem}>{error}</li>
                  ))}
                </ul>
              </div>
            )}
            <form onSubmit={handleRegister} className={registerStyles.form}>
              <div className={registerStyles.inputGroup}>
                <input
                  ref={emailInputRef}
                  type="email"
                  placeholder={isStudent ? "Email address (optional)" : "Email address"}
                  value={username}
                  onChange={e => setUsername(e.target.value)}
                  className={cn(
                    registerStyles.input,
                    touched.email && getFieldError('email') && registerStyles['input--error']
                  )}
                  onFocus={(e) => {
                    e.currentTarget.style.borderColor = touched.email && getFieldError('email') ? '#ef4444' : '#3b82f6';
                  }}
                  onBlur={(e) => {
                    handleFieldTouch('email');
                    e.currentTarget.style.borderColor = touched.email && getFieldError('email') ? '#ef4444' : '#d1d5db';
                  }}
                />
                {touched.email && getFieldError('email') && (
                  <div className={registerStyles.fieldError}>{getFieldError('email')}</div>
                )}
              </div>

              <div className={registerStyles.inputGroup}>
                <input
                  type="text"
                  placeholder="Full name"
                  value={fullName}
                  onChange={e => setFullName(e.target.value)}
                  className={cn(
                    registerStyles.input,
                    touched.fullName && getFieldError('fullName') && registerStyles['input--error']
                  )}
                  onFocus={(e) => {
                    e.currentTarget.style.borderColor = touched.fullName && getFieldError('fullName') ? '#ef4444' : '#3b82f6';
                  }}
                  onBlur={(e) => {
                    handleFieldTouch('fullName');
                    e.currentTarget.style.borderColor = touched.fullName && getFieldError('fullName') ? '#ef4444' : '#d1d5db';
                  }}
                />
                {touched.fullName && getFieldError('fullName') && (
                  <div className={registerStyles.fieldError}>{getFieldError('fullName')}</div>
                )}
              </div>

              <div className={registerStyles.inputGroup}>
                <input
                  type="date"
                  placeholder="Date of Birth (optional)"
                  value={dateOfBirth}
                  onChange={e => setDateOfBirth(e.target.value)}
                  className={registerStyles.input}
                  style={{
                    color: dateOfBirth ? '#111827' : '#9ca3af',
                  }}
                  onFocus={(e) => {
                    e.currentTarget.style.borderColor = '#3b82f6';
                  }}
                  onBlur={(e) => {
                    handleFieldTouch('dateOfBirth');
                    e.currentTarget.style.borderColor = '#d1d5db';
                  }}
                />
                <div className={registerStyles.optionalFieldHint}>Date of Birth (optional)</div>
              </div>

              {isStudent && (
                <div className={registerStyles.inputGroup}>
                  <select
                    value={selectedClassLevelId}
                    onChange={e => setSelectedClassLevelId(e.target.value)}
                    className={cn(
                      registerStyles.input,
                      touched.classLevelId && getFieldError('classLevelId') && registerStyles['input--error']
                    )}
                    onFocus={(e) => {
                      e.currentTarget.style.borderColor = touched.classLevelId && getFieldError('classLevelId') ? '#ef4444' : '#3b82f6';
                    }}
                    onBlur={(e) => {
                      handleFieldTouch('classLevelId');
                      e.currentTarget.style.borderColor = touched.classLevelId && getFieldError('classLevelId') ? '#ef4444' : '#d1d5db';
                    }}
                    disabled={classLoading}
                  >
                    <option value="">Select your class</option>
                    {classLevels.map(level => (
                      <option key={level.id} value={level.id}>
                        {level.name}
                      </option>
                    ))}
                  </select>
                  {touched.classLevelId && getFieldError('classLevelId') && (
                    <div className={registerStyles.fieldError}>{getFieldError('classLevelId')}</div>
                  )}
                  {classLoading && <div className={registerStyles.optionalFieldHint}>Loading classes...</div>}
                </div>
              )}

              {!isStudent && (
                <>
                  <div className={registerStyles.inputGroup}>
                <PasswordInput
                placeholder="Password"
                value={password}
                onChange={e => {
                    setPassword(e.target.value);
                    // Show password info when user starts typing
                    if (e.target.value.length > 0 && !showPasswordInfo) {
                      setShowPasswordInfo(true);
                    }
                  }}
                className={cn(
                    registerStyles.input,
                    touched.password && getFieldError('password') && registerStyles['input--error']
                  )}
                onFocus={(e) => {
                    e.currentTarget.style.borderColor = touched.password && getFieldError('password') ? '#ef4444' : '#3b82f6';
                    // Also show password info when user focuses on password field
                    if (password.length > 0 || !showPasswordInfo) {
                      setShowPasswordInfo(true);
                    }
                  }}
                onBlur={(e) => {
                    handleFieldTouch('password');
                    e.currentTarget.style.borderColor = touched.password && getFieldError('password') ? '#ef4444' : '#d1d5db';
                  }}
              />
                {touched.password && getFieldError('password') && (
                  <div className={registerStyles.fieldError}>{getFieldError('password')}</div>
                )}
              </div>

              <div className={registerStyles.inputGroup}>
                <PasswordInput
                placeholder="Confirm password"
                value={confirmPassword}
                onChange={e => setConfirmPassword(e.target.value)}
                className={cn(
                    registerStyles.input,
                    touched.confirmPassword && getFieldError('confirmPassword') && registerStyles['input--error']
                  )}
                onFocus={(e) => {
                    e.currentTarget.style.borderColor = touched.confirmPassword && getFieldError('confirmPassword') ? '#ef4444' : '#3b82f6';
                  }}
                onBlur={(e) => {
                    handleFieldTouch('confirmPassword');
                    e.currentTarget.style.borderColor = touched.confirmPassword && getFieldError('confirmPassword') ? '#ef4444' : '#d1d5db';
                  }}
              />
                {touched.confirmPassword && getFieldError('confirmPassword') && (
                  <div className={registerStyles.fieldError}>{getFieldError('confirmPassword')}</div>
                )}
              </div>
                </>
              )}

          {/* Error Messages */}
          {errors.length > 0 && (
            <div className={registerStyles.errorContainer}>
              {errors.map((error, i) => (
                <div key={i} className={registerStyles.errorMessage}>{error}</div>
              ))}
            </div>
          )}

              {/* Password Policy Information - Only show when user starts typing password */}
              {showPasswordInfo && (
                <div className={registerStyles.passwordInfo}>
                  <h4 className={registerStyles.passwordInfoTitle}>Password Requirements:</h4>
                  {policyLoading ? (
                    <p className={registerStyles.loadingText}>Loading password requirements...</p>
                  ) : passwordPolicy ? (
                    <ul className={registerStyles.passwordInfoList}>
                      <li className={registerStyles.passwordInfoItem}>
                        Minimum length: <strong>{passwordPolicy.requiredLength} characters</strong>
                      </li>
                      {passwordPolicy.requireDigit && (
                        <li className={registerStyles.passwordInfoItem}>Must contain at least one digit (0-9)</li>
                      )}
                      {passwordPolicy.requireUppercase && (
                        <li className={registerStyles.passwordInfoItem}>Must contain at least one uppercase letter (A-Z)</li>
                      )}
                      {passwordPolicy.requireLowercase && (
                        <li className={registerStyles.passwordInfoItem}>Must contain at least one lowercase letter (a-z)</li>
                      )}
                      {passwordPolicy.requireNonAlphanumeric && (
                        <li className={registerStyles.passwordInfoItem}>Must contain at least one special character (!@#$%^&* etc.)</li>
                      )}
                    </ul>
                  ) : (
                    <p className={registerStyles.passwordInfoError}>Failed to load password requirements. Please ensure your password is at least 6 characters long.</p>
                  )}
                </div>
              )}

          <button
            type="submit"
            className={cn(registerStyles.submitButton, {
              [registerStyles.submitButtonDisabled]: isSubmitting
            })}
            disabled={isSubmitting}
            title="Press Ctrl+Enter to create account quickly"
          >
            {isSubmitting ? 'Creating Account...' : 'Create Account'}
          </button>
        </form>
      </>
          )
        ) : null}

        <div className={registerStyles.loginLink}>
          Already have an account?{' '}
          <a href="/login" className={registerStyles.link}>Sign in here</a>
        </div>
      </div>
    </div>
  );
};

export default Register;