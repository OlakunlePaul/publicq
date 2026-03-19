import React, { useEffect, useState } from 'react';
import UserManagement from '../components/UserManagement/UserManagement';
import GroupManagement from '../components/GroupManagement/GroupManagement';
import ModuleManagement from '../components/ModuleManagement/ModuleManagement';
import EmailManagement from '../components/EmailManagement/EmailManagement';
import BannerManagement from '../components/BannerManagement/BannerManagement';
import PageManagement from '../components/PageManagement/PageManagement';
import AiConfiguration from '../components/AiConfiguration/AiConfiguration';
import AiChatDemo from './AiChat';
import TokenManagement from '../components/TokenManagement/TokenManagement';
import PasswordManagement from '../components/PasswordManagement/PasswordManagement';
import UserRegistrationManagement from '../components/UserRegistrationManagement/UserRegistrationManagement';
import AdmissionNumberManagement from '../components/AdmissionNumberManagement/AdmissionNumberManagement';
import CacheManagement from '../components/CacheManagement/CacheManagement';
import FileStorageManagement from '../components/FileStorageManagement/FileStorageManagement';
import LogManagement from '../components/LogManagement/LogManagement';
import IpRateLimiting from '../components/IpRateLimiting/IpRateLimiting';
import AssignmentManagement from '../components/AssignmentManagement/AssignmentManagement';
import ReportsAnalytics from '../components/ReportsAnalytics/ReportsAnalytics';
import AcademicStructureManagement from '../components/AcademicStructureManagement/AcademicStructureManagement';
import ResultManagement from '../components/ResultManagement/ResultManagement';
import SchoolBrandingManagement from '../components/Admin/Settings/SchoolBrandingManagement';
import PermissionManagement from '../components/Admin/Permissions/PermissionManagement';
import { PlatformStatisticService } from '../services/platformStatisticService';
import { User } from '../models/user';
import { Group } from '../models/group';
import { Assignment } from '../models/assignment';
import { AssessmentModuleDto } from '../models/assessment-module';
import { MessageProvider } from '../models/MessageProvider';
import { useAuth } from '../context/AuthContext';
import { UserPolicies } from '../models/user-policy';
import { cn } from '../utils/cn';
import cssStyles from './Admin.module.css';

type AdminSection = 'dashboard' | 'users' | 'groups' | 'assignments' | 'assessments' | 'reports' | 'email' | 'banners' | 'pages' | 'ai' | 'ai-chat' | 'security' | 'cache' | 'storage' | 'logs' | 'admissions' | 'branding' | 'academic' | 'results' | 'permissions';

// Animated Counter Component
const AnimatedCounter = ({ target, duration = 1000, delay = 0 }: { target: number; duration?: number; delay?: number }) => {
  const [count, setCount] = useState(0);
  const [hasStarted, setHasStarted] = useState(false);

  useEffect(() => {
    const startAnimation = () => {
      setHasStarted(true);
      let startTimestamp: number | null = null;

      const step = (timestamp: number) => {
        if (!startTimestamp) startTimestamp = timestamp;
        const progress = Math.min((timestamp - startTimestamp) / duration, 1);

        // Easing function for smooth animation
        const easeOutQuart = 1 - Math.pow(1 - progress, 4);

        setCount(Math.floor(easeOutQuart * target));

        if (progress < 1) {
          window.requestAnimationFrame(step);
        } else {
          setCount(target); // Ensure we end exactly at target
        }
      };

      window.requestAnimationFrame(step);
    };

    if (target > 0) {
      if (delay > 0) {
        setTimeout(startAnimation, delay);
      } else {
        startAnimation();
      }
    }
  }, [target, duration, delay]);

  return <span style={{ opacity: hasStarted ? 1 : 0.3, transition: 'opacity 0.3s ease-in-out' }}>{count}</span>;
};

const Admin = () => {
  const { userRoles } = useAuth();
  const [activeSection, setActiveSection] = useState<AdminSection>('dashboard');
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [userCount, setUserCount] = useState<number>(0);
  const [groupCount, setGroupCount] = useState<number>(0);
  const [moduleCount, setModuleCount] = useState<number>(0);
  const [assignmentCount, setAssignmentCount] = useState<number>(0);
  const [questionCount, setQuestionCount] = useState<number>(0);
  const [dashboardDataLoaded, setDashboardDataLoaded] = useState<boolean>(false);
  const [dashboardLoading, setDashboardLoading] = useState<boolean>(false);
  const [dashboardError, setDashboardError] = useState<string>('');

  // User management state
  const [userManagementData, setUserManagementData] = useState({
    users: [] as User[],
    totalPages: 1,
    currentPage: 1,
    dataLoaded: false,
  });

  // Group management state
  const [groupManagementData, setGroupManagementData] = useState({
    groups: [] as Group[],
    totalPages: 1,
    currentPage: 1,
    dataLoaded: false,
  });

  // Assignment management state
  const [assignmentManagementData, setAssignmentManagementData] = useState({
    assignments: [] as Assignment[],
    totalPages: 1,
    currentPage: 1,
    dataLoaded: false,
  });

  // Module management state
  const [moduleManagementData, setModuleManagementData] = useState({
    modules: [] as AssessmentModuleDto[],
    totalPages: 1,
    currentPage: 1,
    dataLoaded: false,
  });

  // Email configuration
  const [emailOptions, setEmailOptions] = useState({
    enabled: false,
    messageProvider: MessageProvider.Sendgrid,
    sendFrom: '',
    dataLoaded: false,
  });

  // Token configuration
  const [tokenOptions, setTokenOptions] = useState({
    jwtSettings: {
      secret: '',
      issuer: '',
      audience: '',
      tokenExpiryMinutes: undefined as number | undefined,
    } as {
      secret: string;
      issuer: string;
      audience: string;
      tokenExpiryMinutes?: number;
    },
    dataLoaded: false,
  });

  // Password policy configuration
  const [passwordOptions, setPasswordOptions] = useState({
    requiredLength: 6,
    requireDigit: false,
    requireUppercase: false,
    requireLowercase: false,
    requireNonAlphanumeric: false,
    dataLoaded: false,
  });

  // User registration configuration
  const [userRegistrationOptions, setUserRegistrationOptions] = useState({
    enabled: false,
    dataLoaded: false,
  });

  // Admission Number configuration
  const [admissionNumberOptions, setAdmissionNumberOptions] = useState({
    format: 'EN-{YYYY}-{0000}',
    lastSequenceNumber: 0,
    dataLoaded: false,
  });

  // Cache configuration
  const [cacheOptions, setCacheOptions] = useState({
    enable: false,
    connectionString: '',
    keyPrefix: '',
    reportCacheDurationInMinutes: 60,
    dataLoaded: false,
  });

  // Log configuration
  const [logOptions, setLogOptions] = useState({
    enable: true,
    logLevel: 'Information' as any,
    retentionPeriodInDays: 30,
    dataLoaded: false,
  });

  // File storage configuration
  const [fileStorageOptions, setFileStorageOptions] = useState({
    maxSizeKb: 0,
    dataLoaded: false,
  });

  // Hash-based navigation setup
  useEffect(() => {
    const handleHashChange = () => {
      const hash = window.location.hash.slice(1) as AdminSection || 'dashboard';
      setActiveSection(hash);
    };

    window.addEventListener('hashchange', handleHashChange);

    const initialHash = window.location.hash.slice(1) as AdminSection || 'dashboard';
    setActiveSection(initialHash);

    return () => {
      window.removeEventListener('hashchange', handleHashChange);
    };
  }, []);

  const navigateToSection = (section: AdminSection) => {
    window.location.hash = section;
  };

  const getSectionInfo = (section: AdminSection) => {
    const sectionMap = {
      dashboard: { title: 'Dashboard', icon: <img src="https://cdn-icons-png.flaticon.com/512/1828/1828762.png" alt="Dashboard" style={{width: '24px', height: '24px'}} />, description: 'Overview and statistics' },
      users: { title: 'User Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/3126/3126647.png" alt="Users" style={{width: '24px', height: '24px'}} />, description: 'Manage users and permissions' },
      groups: { title: 'Group Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/615/615075.png" alt="Groups" style={{width: '24px', height: '24px'}} />, description: 'Organize users into groups' },
      assignments: { title: 'Assignment Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="Assignments" style={{width: '24px', height: '24px'}} />, description: 'Create and manage assignments' },
      assessments: { title: 'Exam Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/3429/3429153.png" alt="Exams" style={{width: '24px', height: '24px'}} />, description: 'Create and manage exams' },
      reports: { title: 'Reports & Analytics', icon: <img src="https://cdn-icons-png.flaticon.com/512/423/423794.png" alt="Reports" style={{width: '24px', height: '24px'}} />, description: 'View reports and analytics' },
      email: { title: 'Email Configuration', icon: <img src="https://cdn-icons-png.flaticon.com/512/732/732200.png" alt="Email" style={{width: '24px', height: '24px'}} />, description: 'Configure email settings' },
      banners: { title: 'Banner Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/1997/1997842.png" alt="Banners" style={{width: '24px', height: '24px'}} />, description: 'Manage site-wide banners' },
      pages: { title: 'Page Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="Pages" style={{width: '24px', height: '24px'}} />, description: 'Manage static pages' },
      ai: { title: 'AI Settings', icon: <img src="https://cdn-icons-png.flaticon.com/512/2040/2040523.png" alt="AI" style={{width: '24px', height: '24px'}} />, description: 'Configure AI Monkey settings' },
      'ai-chat': { title: 'AI Chat', icon: <img src="https://cdn-icons-png.flaticon.com/512/134/134914.png" alt="Chat" style={{width: '24px', height: '24px'}} />, description: 'Chat with AI Monkey' },
      security: { title: 'Security Settings', icon: <img src="https://cdn-icons-png.flaticon.com/512/3064/3064155.png" alt="Security" style={{width: '24px', height: '24px'}} />, description: 'Manage security and authentication' },
      cache: { title: 'Cache Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/2874/2874802.png" alt="Cache" style={{width: '24px', height: '24px'}} />, description: 'Manage application cache' },
      storage: { title: 'File Storage', icon: <img src="https://cdn-icons-png.flaticon.com/512/2965/2965312.png" alt="Storage" style={{width: '24px', height: '24px'}} />, description: 'Configure file storage settings' },
      logs: { title: 'Log Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/1069/1069159.png" alt="Logs" style={{width: '24px', height: '24px'}} />, description: 'View and manage system logs' },
      admissions: { title: 'Admission Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="Admissions" style={{width: '24px', height: '24px'}} />, description: 'Configure student admission number format' },
      branding: { title: 'School Branding', icon: <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="Branding" style={{width: '24px', height: '24px'}} />, description: 'Configure school identity' },
      academic: { title: 'Academic Structure', icon: <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="Academic" style={{width: '24px', height: '24px'}} />, description: 'Manage Sessions, Terms, Classes, and Subjects' },
      results: { title: 'Result Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="Results" style={{width: '24px', height: '24px'}} />, description: 'Manage bulk scores, ranking, and moderation' },
      permissions: { title: 'Permission Management', icon: <img src="https://cdn-icons-png.flaticon.com/512/3064/3064155.png" alt="Permissions" style={{width: '24px', height: '24px'}} />, description: 'Configure granular access for each role' }
    };
    return sectionMap[section] || sectionMap.dashboard;
  };

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.shiftKey && (e.ctrlKey || e.metaKey)) {
        switch (e.key.toUpperCase()) {
          case 'D': e.preventDefault(); navigateToSection('dashboard'); break;
          case 'U': if (UserPolicies.hasManagerAccess(userRoles)) { e.preventDefault(); navigateToSection('users'); } break;
          case 'A': if (UserPolicies.hasManagerAccess(userRoles)) { e.preventDefault(); navigateToSection('assignments'); } break;
          case 'M': if (UserPolicies.hasContributorAccess(userRoles)) { e.preventDefault(); navigateToSection('assessments'); } break;
          case 'G': if (UserPolicies.hasContributorAccess(userRoles)) { e.preventDefault(); navigateToSection('groups'); } break;
          case 'E': if (UserPolicies.hasAdminAccess(userRoles)) { e.preventDefault(); navigateToSection('email'); } break;
          case 'I': if (UserPolicies.hasAdminAccess(userRoles)) { e.preventDefault(); navigateToSection('ai'); } break;
          case 'K': if (UserPolicies.hasContributorAccess(userRoles)) { e.preventDefault(); navigateToSection('ai-chat'); } break;
          case 'S': if (UserPolicies.hasAdminAccess(userRoles)) { e.preventDefault(); navigateToSection('security'); } break;
          case 'C': if (UserPolicies.hasAdminAccess(userRoles)) { e.preventDefault(); navigateToSection('cache'); } break;
          case 'F': if (UserPolicies.hasAdminAccess(userRoles)) { e.preventDefault(); navigateToSection('storage'); } break;
          case 'L': if (UserPolicies.hasManagerAccess(userRoles)) { e.preventDefault(); navigateToSection('logs'); } break;
          case 'R': if (UserPolicies.hasManagerAccess(userRoles)) { e.preventDefault(); navigateToSection('reports'); } break;
        }
      }
    };
    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [userRoles]);

  useEffect(() => {
    const fetchDashboardData = async () => {
      if (!dashboardDataLoaded && !dashboardLoading) {
        setDashboardLoading(true);
        setDashboardError('');
        try {
          const response = await PlatformStatisticService.getPlatformStatistics();
          if (response.isSuccess && response.data) {
            setUserCount(response.data.totalUsers);
            setGroupCount(response.data.totalGroups);
            setModuleCount(response.data.totalModules);
            setAssignmentCount(response.data.totalAssignments);
            setQuestionCount(response.data.totalQuestions);
            setDashboardDataLoaded(true);
          } else {
            throw new Error('Failed to fetch platform statistics');
          }
        } catch (error) {
          setDashboardError('Failed to load dashboard data. Please try refreshing the page.');
        } finally {
          setDashboardLoading(false);
        }
      }
    };
    fetchDashboardData();
  }, [dashboardDataLoaded, dashboardLoading]);

  const renderContent = () => {
    switch (activeSection) {
      case 'users': return <UserManagement userManagementData={userManagementData} setUserManagementData={setUserManagementData} currentUserRoles={userRoles} />;
      case 'groups': return <GroupManagement groupManagementData={groupManagementData} setGroupManagementData={setGroupManagementData} />;
      case 'assignments': return <AssignmentManagement assignmentManagementData={assignmentManagementData} setAssignmentManagementData={setAssignmentManagementData} />;
      case 'assessments': return <ModuleManagement moduleManagementData={moduleManagementData} setModuleManagementData={setModuleManagementData} onNavigateToGroups={() => navigateToSection('groups')} />;
      case 'email': return <EmailManagement emailConfig={emailOptions} setEmailConfig={setEmailOptions} />;
      case 'banners': return <BannerManagement />;
      case 'pages': return <PageManagement />;
      case 'ai': return <AiConfiguration />;
      case 'ai-chat': return <AiChatDemo onNavigateToSettings={() => navigateToSection('ai')} hideHeader={true} />;
      case 'admissions': return <AdmissionNumberManagement admissionConfig={admissionNumberOptions} setAdmissionConfig={setAdmissionNumberOptions} />;
      case 'security': return (
        <div>
          <UserRegistrationManagement userRegistrationConfig={userRegistrationOptions} setUserRegistrationConfig={setUserRegistrationOptions} />
          <TokenManagement tokenConfig={tokenOptions} setTokenConfig={setTokenOptions} />
          <PasswordManagement passwordConfig={passwordOptions} setPasswordConfig={setPasswordOptions} />
          <IpRateLimiting />
        </div>
      );
      case 'cache': return <CacheManagement cacheConfig={cacheOptions} setCacheConfig={setCacheOptions} />;
      case 'storage': return <FileStorageManagement fileStorageConfig={fileStorageOptions} setFileStorageConfig={setFileStorageOptions} />;
      case 'logs': return <LogManagement logConfig={logOptions} setLogConfig={setLogOptions} />;
      case 'dashboard': return <DashboardContent userCount={userCount} groupCount={groupCount} moduleCount={moduleCount} assignmentCount={assignmentCount} questionCount={questionCount} loading={dashboardLoading} error={dashboardError} onNavigate={navigateToSection} />;
      case 'reports': return <ReportsAnalytics />;
      case 'academic': return <AcademicStructureManagement />;
      case 'results': return <ResultManagement />;
      case 'branding': return <SchoolBrandingManagement />;
      case 'permissions': return <PermissionManagement />;
      default: return <DashboardContent userCount={userCount} groupCount={groupCount} moduleCount={moduleCount} assignmentCount={assignmentCount} questionCount={questionCount} loading={dashboardLoading} error={dashboardError} onNavigate={navigateToSection} />;
    }
  };

  return (
    <div className={cssStyles.container}>
      <div className={cssStyles.sidebar}>
        <nav className={cssStyles.nav}>
          <button onClick={() => navigateToSection('dashboard')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'dashboard' })}>
            <img src="https://cdn-icons-png.flaticon.com/512/1828/1828762.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Dashboard
          </button>
          {UserPolicies.hasAdminAccess(userRoles) && (
            <button onClick={() => navigateToSection('ai')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'ai' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/2040/2040523.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> AI Settings
            </button>
          )}
          {UserPolicies.hasContributorAccess(userRoles) && (
            <button onClick={() => navigateToSection('ai-chat')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'ai-chat' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/134/134914.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> AI Chat
            </button>
          )}
          {UserPolicies.hasManagerAccess(userRoles) && (
            <button onClick={() => navigateToSection('assignments')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'assignments' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Assignments
            </button>
          )}
          {UserPolicies.hasManagerAccess(userRoles) && (
            <button onClick={() => navigateToSection('banners')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'banners' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/1997/1997842.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Banners
            </button>
          )}
          {UserPolicies.hasManagerAccess(userRoles) && (
            <button onClick={() => navigateToSection('pages')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'pages' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Pages
            </button>
          )}
          {UserPolicies.hasAdminAccess(userRoles) && (
            <button onClick={() => navigateToSection('cache')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'cache' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/2874/2874802.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Cache
            </button>
          )}
          {UserPolicies.hasAdminAccess(userRoles) && (
            <button onClick={() => navigateToSection('email')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'email' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/732/732200.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Email
            </button>
          )}
          {UserPolicies.hasAdminAccess(userRoles) && (
            <button onClick={() => navigateToSection('storage')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'storage' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/2965/2965312.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> File Storage
            </button>
          )}
          {UserPolicies.hasContributorAccess(userRoles) && (
            <button onClick={() => navigateToSection('groups')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'groups' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/615/615075.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Groups
            </button>
          )}
          {UserPolicies.hasManagerAccess(userRoles) && (
            <button onClick={() => navigateToSection('logs')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'logs' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/1069/1069159.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Logs
            </button>
          )}
          {UserPolicies.hasContributorAccess(userRoles) && (
            <button onClick={() => navigateToSection('assessments')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'assessments' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/3429/3429153.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Exams
            </button>
          )}
          {UserPolicies.hasManagerAccess(userRoles) && (
            <button onClick={() => navigateToSection('reports')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'reports' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/423/423794.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Reports & Analytics
            </button>
          )}
          {UserPolicies.hasAdminAccess(userRoles) && (
            <button onClick={() => navigateToSection('security')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'security' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/3064/3064155.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Security
            </button>
          )}
          {UserPolicies.hasAdminAccess(userRoles) && (
            <button onClick={() => navigateToSection('permissions')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'permissions' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/3064/3064155.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Permissions
            </button>
          )}
          {UserPolicies.hasManagerAccess(userRoles) && (
            <button onClick={() => navigateToSection('users')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'users' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/3126/3126647.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Users
            </button>
          )}
          {UserPolicies.hasManagerAccess(userRoles) && (
            <button onClick={() => navigateToSection('admissions')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'admissions' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Admissions
            </button>
          )}
          {UserPolicies.hasManagerAccess(userRoles) && (
            <button onClick={() => navigateToSection('branding')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'branding' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> School Branding
            </button>
          )}
          {/* Note: In a real system you'd use a specific policy for Academic/Results. Using Contributor/Manager for now */}
          {UserPolicies.hasManagerAccess(userRoles) && (
            <button onClick={() => navigateToSection('academic')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'academic' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Academic
            </button>
          )}
          {UserPolicies.hasContributorAccess(userRoles) && (
            <button onClick={() => navigateToSection('results')} className={cn(cssStyles.navButton, { [cssStyles.activeNavButton]: activeSection === 'results' })}>
              <img src="https://cdn-icons-png.flaticon.com/512/2991/2991106.png" alt="" style={{width: '18px', height: '18px', marginRight: '12px'}} /> Results
            </button>
          )}
        </nav>
      </div>
      
      <div className={cssStyles.content}>
        <div className={cssStyles.sectionHeader}>
          <div className={cssStyles.sectionInfo}>
            <div className={cssStyles.sectionText}>
              <h2 className={cssStyles.sectionTitle}>
                {getSectionInfo(activeSection).icon} {getSectionInfo(activeSection).title}
              </h2>
              <p className={cssStyles.sectionDescription}>{getSectionInfo(activeSection).description}</p>
            </div>
          </div>
          
          <div className={cssStyles.sectionDropdown}>
            <button onClick={() => setIsDropdownOpen(!isDropdownOpen)} className={cssStyles.sectionSelectButton} style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '10px 16px', border: '1px solid rgba(226, 232, 240, 0.8)', borderRadius: '12px', background: 'white', cursor: 'pointer', minWidth: '220px', fontWeight: 600 }}>
              {getSectionInfo(activeSection).icon}
              <span style={{ flex: 1, textAlign: 'left' }}>{getSectionInfo(activeSection).title}</span>
              <svg width="12" height="12" viewBox="0 0 12 12" fill="none"><path d="M2 4L6 8L10 4" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/></svg>
            </button>
            {isDropdownOpen && (
              <div style={{ position: 'absolute', top: '100%', right: 0, background: 'white', border: '1px solid rgba(226, 232, 240, 0.8)', borderRadius: '16px', marginTop: '12px', boxShadow: '0 20px 25px -5px rgba(0,0,0,0.1)', zIndex: 1000, overflow: 'hidden', padding: '8px', minWidth: '220px' }}>
                {['dashboard', 'users', 'admissions', 'branding', 'permissions', 'groups', 'assignments', 'assessments', 'reports', 'email', 'banners', 'ai', 'ai-chat', 'security', 'logs'].map(section => (
                  <button key={section} onClick={() => { navigateToSection(section as AdminSection); setIsDropdownOpen(false); }} style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '10px 12px', width: '100%', border: 'none', background: 'transparent', borderRadius: '8px', cursor: 'pointer', textAlign: 'left', fontWeight: activeSection === section ? 600 : 400, color: activeSection === section ? '#4f46e5' : '#475569' }}>
                    {getSectionInfo(section as AdminSection).icon}
                    <span>{getSectionInfo(section as AdminSection).title}</span>
                  </button>
                ))}
              </div>
            )}
          </div>
        </div>
        {renderContent()}
      </div>
    </div>
  );
};

const DashboardContent = ({ userCount, groupCount, moduleCount, assignmentCount, questionCount, loading, error, onNavigate }: any) => {
  const { userRoles } = useAuth();
  
  if (loading) return (
    <div className={cssStyles.dashboardContainer}>
      <div className={cssStyles.loadingContainer}><p className={cssStyles.loadingText}>Loading dashboard analytics...</p></div>
    </div>
  );

  if (error) return (
    <div className={cssStyles.dashboardContainer}>
      <div className={cssStyles.errorContainer}><p className={cssStyles.errorText}>{error}</p></div>
    </div>
  );

  return (
    <div className={cssStyles.dashboardContainer}>
      <div className={cssStyles.statsGrid}>
        <div className={cssStyles.statCard} style={{ animationDelay: '0.1s' }}>
          <h3>Users</h3>
          <p className={cssStyles.statNumber}><AnimatedCounter target={userCount} delay={100} /></p>
        </div>
        <div className={cssStyles.statCard} style={{ animationDelay: '0.2s' }}>
          <h3>Groups</h3>
          <p className={cssStyles.statNumber}><AnimatedCounter target={groupCount} delay={300} /></p>
        </div>
        <div className={cssStyles.statCard} style={{ animationDelay: '0.3s' }}>
          <h3>Assignments</h3>
          <p className={cssStyles.statNumber}><AnimatedCounter target={assignmentCount} delay={500} /></p>
        </div>
        <div className={cssStyles.statCard} style={{ animationDelay: '0.4s' }}>
          <h3>Exams</h3>
          <p className={cssStyles.statNumber}><AnimatedCounter target={moduleCount} delay={700} /></p>
        </div>
        <div className={cssStyles.statCard} style={{ animationDelay: '0.5s' }}>
          <h3>Questions</h3>
          <p className={cssStyles.statNumber}><AnimatedCounter target={questionCount} delay={900} /></p>
        </div>
      </div>
      
      <div className={cssStyles.welcomeMessage}>
        <p>Welcome to the ExamNova Admin Panel. Your central hub for managing assessments, users, and platform performance.</p>
        <div className={cssStyles.statisticsNote}>
          <p className={cssStyles.statisticsNoteText}>
            <img src="https://cdn-icons-png.flaticon.com/512/189/189665.png" alt="Info" style={{width: '18px', height: '18px'}} />
            Statistics are synchronized with live background indexing.
          </p>
        </div>
        
        <div className={cssStyles.shortcutsSection}>
          <div className={cssStyles.shortcutsHeader}><span className={cssStyles.shortcutsTitle}>Quick Shortcuts</span></div>
          <div className={cssStyles.shortcutsList}>
            <div className={cssStyles.shortcutItem}><span className={cssStyles.shortcutLabel}>Dashboard</span><span className={cssStyles.shortcutKeys}>Ctrl+Shift+D</span></div>
            {UserPolicies.hasManagerAccess(userRoles) && <div className={cssStyles.shortcutItem}><span className={cssStyles.shortcutLabel}>Users</span><span className={cssStyles.shortcutKeys}>Ctrl+Shift+U</span></div>}
            {UserPolicies.hasManagerAccess(userRoles) && <div className={cssStyles.shortcutItem}><span className={cssStyles.shortcutLabel}>Assignments</span><span className={cssStyles.shortcutKeys}>Ctrl+Shift+A</span></div>}
            {UserPolicies.hasContributorAccess(userRoles) && <div className={cssStyles.shortcutItem}><span className={cssStyles.shortcutLabel}>Exams</span><span className={cssStyles.shortcutKeys}>Ctrl+Shift+M</span></div>}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Admin;
