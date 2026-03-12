import LoginPage from './pages/Login';
import './App.css';
import { Link, Route, Routes, BrowserRouter, useLocation } from 'react-router-dom';
import Register from './pages/Register';
import ResetPassword from './pages/ResetPassword';
import NavBar from './components/NavBar/NavBar';
import Banner from './components/Banner/Banner';
import { AuthProvider, useAuth } from './context/AuthContext';
import Admin from './pages/Admin';
import ModuleCreationPage from './pages/ModuleCreationPage';
import ModuleBuilderPage from './pages/ModuleBuilderPage';
import { RoleGuard } from './components/Shared/RoleGuard';
import MyAssignments from './pages/MyAssignments';
import AssignmentExecutionPage from './pages/AssignmentExecutionPage';
import Questions from './components/Questions';
import { UserPolicies } from './models/user-policy';
import { UserRole } from './models/UserRole';
import homeStyles from './pages/Home/Home.module.css';
import { ROUTES } from './constants/contstants';
import AiChatDemo from './pages/AiChat';
import DemoExam from './components/DemoExam/DemoExam';
import ContactUs from './pages/ContactUs/ContactUs';

function HomePage() {
  const { isAuthenticated, userRoles } = useAuth();
  const isExamTaker = userRoles.includes(UserRole.EXAM_TAKER);

  return (
    <div className={homeStyles.homePage}>
      {/* Hero Section */}
      <section className={homeStyles.hero}>
        <div className={homeStyles.heroContent}>
          <div className={homeStyles.heroBadge}>🎓 Trusted by Academic Institutions</div>
          <h1 className={homeStyles.heroTitle}>
            The Smart Examination Platform for <span className={homeStyles.heroHighlight}>Modern Education</span>
          </h1>
          <p className={homeStyles.heroSubtitle}>
            Create, deliver, and grade assessments seamlessly across primary schools, secondary schools, and universities.
            Empower educators with powerful tools to manage exams efficiently.
          </p>
          <div className={homeStyles.heroActions}>
            <Link to={ROUTES.MY_ASSIGNMENTS} className={homeStyles.btnPrimary}>
              Take an Exam →
            </Link>
            {!isAuthenticated && (
              <Link to={ROUTES.LOGIN} className={homeStyles.btnSecondary}>
                Sign In
              </Link>
            )}
            {!isExamTaker && isAuthenticated && (
              <Link to={ROUTES.MODULE_CREATE} className={homeStyles.btnSecondary}>
                Create Assessment
              </Link>
            )}
          </div>
          <div className={homeStyles.heroStats}>
            <div className={homeStyles.heroStat}>
              <span className={homeStyles.heroStatNumber}>📝</span>
              <span className={homeStyles.heroStatLabel}>Auto-Graded Exams</span>
            </div>
            <div className={homeStyles.heroStatDivider}></div>
            <div className={homeStyles.heroStat}>
              <span className={homeStyles.heroStatNumber}>📊</span>
              <span className={homeStyles.heroStatLabel}>Instant Analytics</span>
            </div>
            <div className={homeStyles.heroStatDivider}></div>
            <div className={homeStyles.heroStat}>
              <span className={homeStyles.heroStatNumber}>🔒</span>
              <span className={homeStyles.heroStatLabel}>Secure & Reliable</span>
            </div>
          </div>
        </div>
      </section>

      {/* Who It's For Section */}
      <section className={homeStyles.audienceSection}>
        <h2 className={homeStyles.sectionTitle}>Built for Every Level of Education</h2>
        <p className={homeStyles.sectionSubtitle}>
          Whether you're running a primary school quiz or a university-level final exam, Examina scales to your needs.
        </p>
        <div className={homeStyles.audienceCards}>
          <div className={homeStyles.audienceCard}>
            <div className={homeStyles.audienceIcon}>🏫</div>
            <h3 className={homeStyles.audienceCardTitle}>Primary Schools</h3>
            <p className={homeStyles.audienceCardDesc}>Simple quizzes with auto-grading. Teachers save hours on marking with instant results.</p>
          </div>
          <div className={homeStyles.audienceCard}>
            <div className={homeStyles.audienceIcon}>🎒</div>
            <h3 className={homeStyles.audienceCardTitle}>Secondary Schools</h3>
            <p className={homeStyles.audienceCardDesc}>Manage class groups, assignments, and track student progress across terms and subjects.</p>
          </div>
          <div className={homeStyles.audienceCard}>
            <div className={homeStyles.audienceIcon}>🎓</div>
            <h3 className={homeStyles.audienceCardTitle}>Universities</h3>
            <p className={homeStyles.audienceCardDesc}>Large-scale exams with multiple versions, timed sessions, randomized questions, and detailed analytics.</p>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className={homeStyles.featuresSection}>
        <h2 className={homeStyles.sectionTitle}>Everything You Need to Run Exams</h2>
        <div className={homeStyles.featuresGrid}>
          <div className={homeStyles.featureCard}>
            <div className={homeStyles.featureCardIcon}>✏️</div>
            <h3 className={homeStyles.featureCardTitle}>Easy Question Builder</h3>
            <p className={homeStyles.featureCardDesc}>Multiple choice, multi-select, and free-text questions with file attachments.</p>
          </div>
          <div className={homeStyles.featureCard}>
            <div className={homeStyles.featureCardIcon}>⚡</div>
            <h3 className={homeStyles.featureCardTitle}>Auto-Grading</h3>
            <p className={homeStyles.featureCardDesc}>Objective questions are graded instantly. Students get results the moment they finish.</p>
          </div>
          <div className={homeStyles.featureCard}>
            <div className={homeStyles.featureCardIcon}>👥</div>
            <h3 className={homeStyles.featureCardTitle}>Class Management</h3>
            <p className={homeStyles.featureCardDesc}>Organize students into groups, assign exams to specific classes, and track completion.</p>
          </div>
          <div className={homeStyles.featureCard}>
            <div className={homeStyles.featureCardIcon}>📈</div>
            <h3 className={homeStyles.featureCardTitle}>Detailed Reports</h3>
            <p className={homeStyles.featureCardDesc}>Per-student scores, question-level analysis, and exportable class performance data.</p>
          </div>
          <div className={homeStyles.featureCard}>
            <div className={homeStyles.featureCardIcon}>🔀</div>
            <h3 className={homeStyles.featureCardTitle}>Anti-Cheating</h3>
            <p className={homeStyles.featureCardDesc}>Randomize question order and answer options. Timed sessions prevent sharing.</p>
          </div>
          <div className={homeStyles.featureCard}>
            <div className={homeStyles.featureCardIcon}>🌐</div>
            <h3 className={homeStyles.featureCardTitle}>Access Anywhere</h3>
            <p className={homeStyles.featureCardDesc}>Students take exams from any device — phone, tablet, or computer. No app install needed.</p>
          </div>
        </div>
      </section>

      {/* How It Works */}
      <section className={homeStyles.howItWorks}>
        <h2 className={homeStyles.sectionTitle}>How It Works</h2>
        <div className={homeStyles.steps}>
          <div className={homeStyles.step}>
            <div className={homeStyles.stepNumber}>1</div>
            <h3 className={homeStyles.stepTitle}>Create</h3>
            <p className={homeStyles.stepDesc}>Build your exam with our intuitive question builder. Add questions, set passing scores, and configure time limits.</p>
          </div>
          <div className={homeStyles.stepConnector}></div>
          <div className={homeStyles.step}>
            <div className={homeStyles.stepNumber}>2</div>
            <h3 className={homeStyles.stepTitle}>Assign</h3>
            <p className={homeStyles.stepDesc}>Create assignments, set exam windows, and add your students. They'll receive their access automatically.</p>
          </div>
          <div className={homeStyles.stepConnector}></div>
          <div className={homeStyles.step}>
            <div className={homeStyles.stepNumber}>3</div>
            <h3 className={homeStyles.stepTitle}>Grade</h3>
            <p className={homeStyles.stepDesc}>Objective questions are graded instantly. Review analytics and export results for your records.</p>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className={homeStyles.ctaSection}>
        <h2 className={homeStyles.ctaTitle}>Ready to Transform Your Exams?</h2>
        <p className={homeStyles.ctaSubtitle}>Join academic institutions using Examina to streamline their assessment process.</p>
        <div className={homeStyles.ctaActions}>
          <Link to={ROUTES.REGISTER} className={homeStyles.btnPrimary}>
            Get Started Free
          </Link>
          <Link to={ROUTES.DEMO} className={homeStyles.btnOutline}>
            Try a Demo Exam
          </Link>
        </div>
      </section>

      {/* Footer */}
      <footer className={homeStyles.footer}>
        <p className={homeStyles.footerText}>
          © 2026 Examina. Smart examination platform for academic institutions.
        </p>
      </footer>
    </div>
  );
}

function AppContent() {
  const location = useLocation();
  const isExamPage = location.pathname.startsWith('/assignment/') || 
                     location.pathname.startsWith(ROUTES.QUESTIONS);

  return (
    <>
      {!isExamPage && <NavBar />}
      {!isExamPage && <Banner />}
      <Routes>
        <Route path={ROUTES.HOME} element={<HomePage />} />
        <Route path={ROUTES.REGISTER} element={<Register />} />
        <Route path={ROUTES.RESET_PASSWORD} element={<ResetPassword />} />
        <Route path={ROUTES.ADMIN} element={
          <RoleGuard requiredRoles={[...UserPolicies.Contributors, ...UserPolicies.Analysts]} redirectTo={ROUTES.LOGIN}>
            <Admin />
          </RoleGuard>
        } />
        <Route path={ROUTES.LOGIN} element={<LoginPage />} />
        <Route path={ROUTES.MY_ASSIGNMENTS} element={<MyAssignments />} />
        <Route path={ROUTES.ASSIGNMENT} element={<AssignmentExecutionPage />} />
        <Route path={ROUTES.QUESTIONS} element={<Questions />} />
        <Route path={ROUTES.MODULE_CREATE} element={
          <RoleGuard requiredRoles={[...UserPolicies.Contributors]} redirectTo={ROUTES.LOGIN}>
            <ModuleCreationPage />
          </RoleGuard>
        } />
        <Route path={ROUTES.MODULE_BUILD} element={
          <RoleGuard requiredRoles={[...UserPolicies.Contributors]} redirectTo={ROUTES.LOGIN}>
            <ModuleBuilderPage />
          </RoleGuard>
        } />
        <Route path={ROUTES.AI_CHAT} element={
          <RoleGuard requiredRoles={[...UserPolicies.Contributors]} redirectTo={ROUTES.LOGIN}>
            <AiChatDemo />
          </RoleGuard>
        } />
        <Route path={ROUTES.DEMO} element={<DemoExam />} />
        <Route path={ROUTES.CONTACT_US} element={<ContactUs />} />
      </Routes>
    </>
  );
}

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;