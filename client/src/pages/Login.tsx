import { useState, useEffect, useCallback, useRef } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import { UserLoginRequest } from '../models/user-login-request';
import { userService } from '../services/userService';
import { cn } from '../utils/cn';
import { getTokenRoles } from '../utils/tokenUtils';
import { UserPolicies } from '../models/user-policy';
import { UserRole } from '../models/UserRole';
import PasswordInput from '../components/Shared/PasswordInput';
import loginStyles from './Login.module.css';

const Login = () => {
  const [loginType, setLoginType] = useState<'staff' | 'student'>('staff');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [admissionId, setAdmissionId] = useState('');
  const [error, setError] = useState('');
  const [touched, setTouched] = useState<{[key: string]: boolean}>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showForgetPassword, setShowForgetPassword] = useState(false);
  const [forgetPasswordEmail, setForgetPasswordEmail] = useState('');
  const [forgetPasswordMessage, setForgetPasswordMessage] = useState('');
  const [isForgetPasswordSubmitting, setIsForgetPasswordSubmitting] = useState(false);
  const { login, loginStudent } = useAuth();
  const navigate = useNavigate();
  const emailInputRef = useRef<HTMLInputElement>(null);
  const studentInputRef = useRef<HTMLInputElement>(null);

  // Auto-focus on input when component mounts or login type changes
  useEffect(() => {
    if (showForgetPassword) return;
    
    if (loginType === 'staff' && emailInputRef.current) {
      emailInputRef.current.focus();
    } else if (loginType === 'student' && studentInputRef.current) {
      studentInputRef.current.focus();
    }
  }, [loginType, showForgetPassword]);

  const getFieldError = (fieldName: string) => {
    if (!touched[fieldName]) return null;
    
    switch (fieldName) {
      case 'email':
        if (!username.trim()) return 'Email is required';
        if (!/\S+@\S+\.\S+/.test(username)) return 'Please enter a valid email address';
        break;
      case 'password':
        if (!password.trim()) return 'Password is required';
        break;
      case 'admissionId':
        if (!admissionId.trim()) return 'Admission Number or ID is required';
        break;
    }
    return null;
  };

  const handleFieldTouch = (fieldName: string) => {
    setTouched({ ...touched, [fieldName]: true });
  };

  const handleLogin = useCallback(async (e?: React.FormEvent) => {
    if (e) e.preventDefault();
    
    if (isSubmitting) return; // Prevent double submission
    
    setError('');

    if (loginType === 'staff') {
      // Mark all fields as touched for validation display
      setTouched({
        email: true,
        password: true,
      });

      // Basic validation
      if (!username.trim() || !password.trim()) {
        setError('Please fill in all fields');
        return;
      }

      if (!/\S+@\S+\.\S+/.test(username)) {
        setError('Please enter a valid email address');
        return;
      }
    } else {
      setTouched({ admissionId: true });
      if (!admissionId.trim()) {
        setError('Please enter your Admission Number or Student ID');
        return;
      }
    }

    setIsSubmitting(true);

    try {
      if (loginType === 'staff') {
        const loginData: UserLoginRequest = {
          email: username,
          password,
        };
        await login(loginData);
      } else {
        await loginStudent(admissionId);
      }
      
      const searchParams = new URLSearchParams(window.location.search);
      const redirectTo = searchParams.get('redirectTo');
      
      if (redirectTo) {
        navigate(redirectTo, { replace: true });
        return;
      }

      const roles = getTokenRoles();
      
      // Prioritize student redirect if logging in as a student
      if (loginType === 'student' || roles.includes(UserRole.EXAM_TAKER)) {
        navigate('/my-exams', { replace: true });
      } else if (UserPolicies.hasContributorAccess(roles) || UserPolicies.hasManagerAccess(roles) || UserPolicies.hasAdminAccess(roles)) {
        navigate('/admin', { replace: true });
      } else if (roles.includes(UserRole.PARENT)) {
        navigate('/parent-dashboard', { replace: true });
      } else {
        navigate('/my-exams', { replace: true });
      }
    } catch (err: any) {
      setError('Login failed: ' + (err.response?.data?.message || err.message));
    } finally {
      setIsSubmitting(false);
    }
  }, [username, password, admissionId, loginType, isSubmitting, login, loginStudent, navigate]);

  const handleForgetPassword = useCallback(async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (isForgetPasswordSubmitting) return;
    
    setError('');
    setForgetPasswordMessage('');

    if (!forgetPasswordEmail.trim()) {
      setError('Please enter your email address');
      return;
    }

    if (!/\S+@\S+\.\S+/.test(forgetPasswordEmail)) {
      setError('Please enter a valid email address');
      return;
    }

    setIsForgetPasswordSubmitting(true);

    try {
      await userService.forgetPassword(forgetPasswordEmail);
      setForgetPasswordMessage('If the email is registered, a password reset link has been sent to your email address.');
      setError('');
    } catch (err: any) {
      setError('Failed to send reset link: ' + (err.response?.data?.message || err.message));
    } finally {
      setIsForgetPasswordSubmitting(false);
    }
  }, [forgetPasswordEmail, isForgetPasswordSubmitting]);

  const handleBackToLogin = () => {
    setShowForgetPassword(false);
    setForgetPasswordEmail('');
    setForgetPasswordMessage('');
    setError('');
    setTouched({});
  };

  const switchLoginType = (type: 'staff' | 'student') => {
    setLoginType(type);
    setError('');
    setTouched({});
  };

  // Handle Ctrl+Enter keyboard shortcut
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
        e.preventDefault();
        handleLogin();
      }
    };

    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [handleLogin]);

  return (
    <div className={loginStyles.container}>
      <div className={loginStyles.loginCard}>
        <div className={loginStyles.header}>
          <h2 className={loginStyles.title}>{showForgetPassword ? 'Reset Password' : 'Welcome Back'}</h2>
          <p className={loginStyles.subtitle}>
            {showForgetPassword 
              ? 'Enter your email to receive a password reset link' 
              : loginType === 'staff' 
                ? 'Sign in to access your assessment modules'
                : 'Enter your ID to access your assessments'
            }
          </p>
        </div>

        {!showForgetPassword && (
          <div className={loginStyles.loginToggle}>
            <button
              type="button"
              className={cn(
                loginStyles.toggleButton,
                loginType === 'staff' && loginStyles.toggleButtonActive
              )}
              onClick={() => switchLoginType('staff')}
            >
              <img src="https://cdn-icons-png.flaticon.com/512/91/91212.png" alt="" className={loginStyles.toggleIcon} />
              Staff Login
            </button>
            <button
              type="button"
              className={cn(
                loginStyles.toggleButton,
                loginType === 'student' && loginStyles.toggleButtonActive
              )}
              onClick={() => switchLoginType('student')}
            >
              <img src="https://cdn-icons-png.flaticon.com/512/354/354637.png" alt="" className={loginStyles.toggleIcon} />
              Student Login
            </button>
          </div>
        )}

        {!showForgetPassword ? (
          <form onSubmit={handleLogin} className={loginStyles.form}>
            {loginType === 'staff' ? (
              <>
                <div className={loginStyles.inputGroup}>
                  <input
                    ref={emailInputRef}
                    type="email"
                    placeholder="Email address"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    className={cn(
                      loginStyles.input,
                      touched.email && getFieldError('email') && loginStyles['input--error']
                    )}
                    onBlur={() => handleFieldTouch('email')}
                  />
                  {touched.email && getFieldError('email') && (
                    <div className={loginStyles.errorMessage}>{getFieldError('email')}</div>
                  )}
                </div>

                <div className={loginStyles.inputGroup}>
                  <PasswordInput
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    className={cn(
                      loginStyles.input,
                      touched.password && getFieldError('password') && loginStyles['input--error']
                    )}
                    onBlur={() => handleFieldTouch('password')}
                  />
                  {touched.password && getFieldError('password') && (
                    <div className={loginStyles.errorMessage}>{getFieldError('password')}</div>
                  )}
                </div>

                <div className={loginStyles.forgotPassword}>
                  <button
                    type="button"
                    onClick={() => setShowForgetPassword(true)}
                    className={loginStyles.forgotPasswordLink}
                  >
                    Forgot password?
                  </button>
                </div>
              </>
            ) : (
              <div className={loginStyles.inputGroup}>
                <input
                  ref={studentInputRef}
                  type="text"
                  placeholder="Admission Number / Student ID"
                  value={admissionId}
                  onChange={(e) => setAdmissionId(e.target.value)}
                  className={cn(
                    loginStyles.input,
                    touched.admissionId && getFieldError('admissionId') && loginStyles['input--error']
                  )}
                  onBlur={() => handleFieldTouch('admissionId')}
                />
                {touched.admissionId && getFieldError('admissionId') && (
                  <div className={loginStyles.errorMessage}>{getFieldError('admissionId')}</div>
                )}
                <p className={loginStyles.subtitle} style={{marginTop: '0.5rem', opacity: 0.8}}>
                  Students do not need a password to login.
                </p>
              </div>
            )}

            <button
              type="submit"
              disabled={isSubmitting}
              className={cn(
                loginStyles.submitButton,
                isSubmitting && loginStyles['submitButton--loading']
              )}
            >
              {isSubmitting ? 'Signing in...' : 'Sign In'}
            </button>
          </form>
        ) : (
          <form onSubmit={handleForgetPassword} className={loginStyles.form}>
            <div className={loginStyles.inputGroup}>
              <input
                type="email"
                placeholder="Email address"
                value={forgetPasswordEmail}
                onChange={(e) => setForgetPasswordEmail(e.target.value)}
                className={loginStyles.input}
                autoFocus
              />
            </div>
            <button
              type="submit"
              disabled={isForgetPasswordSubmitting}
              className={cn(
                loginStyles.submitButton,
                isForgetPasswordSubmitting && loginStyles['submitButton--loading']
              )}
            >
              {isForgetPasswordSubmitting ? 'Sending...' : 'Send Reset Link'}
            </button>
            <button
              type="button"
              onClick={handleBackToLogin}
              className={loginStyles.backButton}
            >
              ← Back to sign in
            </button>
          </form>
        )}

        {error && (
          <div className={loginStyles.errorMessage}>
            {error}
          </div>
        )}

        {forgetPasswordMessage && (
          <div className={loginStyles.successMessage}>
            {forgetPasswordMessage}
          </div>
        )}

        <div className={loginStyles.registerLink}>
          <p className={loginStyles.registerLinkText}>Don't have an account?</p>
          <a href="/register" className={loginStyles.registerLinkButton}>Create one here</a>
        </div>
      </div>
    </div>
  );
};

export default Login;