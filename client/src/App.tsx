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
import MyExams from './pages/MyExams';
import ExamExecutionPage from './pages/ExamExecutionPage';
import Questions from './components/Questions';
import { UserPolicies } from './models/user-policy';
import { UserRole } from './models/UserRole';
import homeStyles from './pages/Home/Home.module.css';
import { ROUTES } from './constants/contstants';
import AiChatDemo from './pages/AiChat';
import DemoExam from './components/DemoExam/DemoExam';
import ContactUs from './pages/ContactUs/ContactUs';
import ParentDashboard from './pages/ParentDashboard';
import { motion, Variants } from 'framer-motion';

const fadeInUp: Variants = {
  hidden: { opacity: 0, y: 30 },
  visible: { opacity: 1, y: 0, transition: { type: 'spring' as const, stiffness: 300, damping: 24 } }
};

const staggerContainer: Variants = {
  hidden: { opacity: 0 },
  visible: { opacity: 1, transition: { staggerChildren: 0.1 } }
};

function HomePage() {
  const { isAuthenticated, userRoles } = useAuth();

  return (
    <div className={homeStyles.homePage}>
      {/* Hero Section */}
      <section className={homeStyles.hero}>
        <motion.div 
          className={homeStyles.heroContent}
          variants={staggerContainer}
          initial="hidden"
          animate="visible"
        >
          <motion.div variants={fadeInUp} className={homeStyles.heroBadge}>
            <img src="https://cdn-icons-png.flaticon.com/512/8074/8074800.png" alt="Trusted" className={homeStyles.heroBadgeIcon} />
            Trusted by Academic Institutions
          </motion.div>
          <motion.h1 variants={fadeInUp} className={homeStyles.heroTitle}>
            The Smart Examination Platform for <span className={homeStyles.heroHighlight}>Modern Education</span>
          </motion.h1>
          <motion.p variants={fadeInUp} className={homeStyles.heroSubtitle}>
            Create, deliver, and grade assessments seamlessly across primary schools, secondary schools, and universities.
            Empower educators with powerful tools to manage exams efficiently.
          </motion.p>
          <motion.div variants={fadeInUp} className={homeStyles.heroActions}>
            {!isAuthenticated && (
              <>
                <Link to={ROUTES.REGISTER} className={homeStyles.btnPrimary}>
                  Get Started Free
                </Link>
                <Link to={ROUTES.LOGIN} className={homeStyles.btnSecondary}>
                  Sign In
                </Link>
              </>
            )}
            
            {isAuthenticated && (UserPolicies.hasContributorAccess(userRoles) || UserPolicies.hasManagerAccess(userRoles) || UserPolicies.hasAdminAccess(userRoles)) && (
              <Link to={ROUTES.ADMIN} className={homeStyles.btnPrimary}>
                Go to Dashboard →
              </Link>
            )}

            {isAuthenticated && userRoles.includes(UserRole.EXAM_TAKER) && !(UserPolicies.hasContributorAccess(userRoles) || UserPolicies.hasManagerAccess(userRoles) || UserPolicies.hasAdminAccess(userRoles)) && (
              <Link to={ROUTES.MY_EXAMS} className={homeStyles.btnPrimary}>
                Go to My Exams →
              </Link>
            )}
            
            {isAuthenticated && userRoles.includes(UserRole.PARENT) && !userRoles.includes(UserRole.EXAM_TAKER) && !(UserPolicies.hasContributorAccess(userRoles) || UserPolicies.hasManagerAccess(userRoles) || UserPolicies.hasAdminAccess(userRoles)) && (
              <Link to={ROUTES.PARENT_DASHBOARD} className={homeStyles.btnPrimary}>
                Go to Parent Dashboard →
              </Link>
            )}
          </motion.div>
          <motion.div variants={fadeInUp} className={homeStyles.heroStats}>
            <div className={homeStyles.heroStat}>
              <img src="https://cdn-icons-png.flaticon.com/512/2991/2991108.png" alt="Auto-Graded" className={homeStyles.heroStatIcon} />
              <span className={homeStyles.heroStatLabel}>Auto-Graded Exams</span>
            </div>
            <div className={homeStyles.heroStatDivider}></div>
            <div className={homeStyles.heroStat}>
              <img src="https://cdn-icons-png.flaticon.com/512/3589/3589030.png" alt="Analytics" className={homeStyles.heroStatIcon} />
              <span className={homeStyles.heroStatLabel}>Instant Analytics</span>
            </div>
            <div className={homeStyles.heroStatDivider}></div>
            <div className={homeStyles.heroStat}>
              <img src="https://cdn-icons-png.flaticon.com/512/3064/3064155.png" alt="Secure" className={homeStyles.heroStatIcon} />
              <span className={homeStyles.heroStatLabel}>Secure & Reliable</span>
            </div>
          </motion.div>
        </motion.div>
      </section>

      {/* Who It's For Section */}
      <section className={homeStyles.audienceSection}>
        <motion.div initial="hidden" whileInView="visible" viewport={{ once: true, amount: 0.2 }} variants={staggerContainer}>
          <motion.h2 variants={fadeInUp} className={homeStyles.sectionTitle}>Built for Every Level of Education</motion.h2>
          <motion.p variants={fadeInUp} className={homeStyles.sectionSubtitle}>
            Whether you're running a primary school quiz or a university-level final exam, ExamNova scales to your needs.
          </motion.p>
          <div className={homeStyles.audienceCards}>
            <motion.div variants={fadeInUp} className={homeStyles.audienceCard}>
              <div className={homeStyles.audienceIcon}>
                <img src="https://cdn-icons-png.flaticon.com/512/167/167707.png" alt="Primary" className={homeStyles.audienceIconImg} />
              </div>
              <h3 className={homeStyles.audienceCardTitle}>Primary Schools</h3>
              <p className={homeStyles.audienceCardDesc}>Simple quizzes with auto-grading. Teachers save hours on marking with instant results.</p>
            </motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.audienceCard}>
              <div className={homeStyles.audienceIcon}>
                <img src="https://cdn-icons-png.flaticon.com/512/2906/2906496.png" alt="Secondary" className={homeStyles.audienceIconImg} />
              </div>
              <h3 className={homeStyles.audienceCardTitle}>Secondary Schools</h3>
              <p className={homeStyles.audienceCardDesc}>Manage class groups, assignments, and track student progress across terms and subjects.</p>
            </motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.audienceCard}>
              <div className={homeStyles.audienceIcon}>
                <img src="https://cdn-icons-png.flaticon.com/512/2490/2490421.png" alt="Universities" className={homeStyles.audienceIconImg} />
              </div>
              <h3 className={homeStyles.audienceCardTitle}>Universities</h3>
              <p className={homeStyles.audienceCardDesc}>Large-scale exams with multiple versions, timed sessions, randomized questions, and detailed analytics.</p>
            </motion.div>
          </div>
        </motion.div>
      </section>

      {/* Features Section */}
      <section className={homeStyles.featuresSection}>
        <motion.div initial="hidden" whileInView="visible" viewport={{ once: true, amount: 0.1 }} variants={staggerContainer}>
          <motion.h2 variants={fadeInUp} className={homeStyles.sectionTitle}>Everything You Need to Run Exams</motion.h2>
          <div className={homeStyles.featuresGrid}>
            <motion.div variants={fadeInUp} className={homeStyles.featureCard}>
              <div className={homeStyles.featureCardIcon}>
                <img src="https://cdn-icons-png.flaticon.com/512/2541/2541991.png" alt="Builder" className={homeStyles.featureCardIconImg} />
              </div>
              <h3 className={homeStyles.featureCardTitle}>Easy Question Builder</h3>
              <p className={homeStyles.featureCardDesc}>Multiple choice, multi-select, and free-text questions with file attachments.</p>
            </motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.featureCard}>
              <div className={homeStyles.featureCardIcon}>
                <img src="https://cdn-icons-png.flaticon.com/512/9513/9513110.png" alt="Auto-Grading" className={homeStyles.featureCardIconImg} />
              </div>
              <h3 className={homeStyles.featureCardTitle}>Auto-Grading</h3>
              <p className={homeStyles.featureCardDesc}>Objective questions are graded instantly. Students get results the moment they finish.</p>
            </motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.featureCard}>
              <div className={homeStyles.featureCardIcon}>
                <img src="https://cdn-icons-png.flaticon.com/512/1077/1077063.png" alt="Class" className={homeStyles.featureCardIconImg} />
              </div>
              <h3 className={homeStyles.featureCardTitle}>Class Management</h3>
              <p className={homeStyles.featureCardDesc}>Organize students into groups, assign exams to specific classes, and track completion.</p>
            </motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.featureCard}>
              <div className={homeStyles.featureCardIcon}>
                <img src="https://cdn-icons-png.flaticon.com/512/2311/2311545.png" alt="Reports" className={homeStyles.featureCardIconImg} />
              </div>
              <h3 className={homeStyles.featureCardTitle}>Detailed Reports</h3>
              <p className={homeStyles.featureCardDesc}>Per-student scores, question-level analysis, and exportable class performance data.</p>
            </motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.featureCard}>
              <div className={homeStyles.featureCardIcon}>
                <img src="https://cdn-icons-png.flaticon.com/512/467/467262.png" alt="Anti-Cheating" className={homeStyles.featureCardIconImg} />
              </div>
              <h3 className={homeStyles.featureCardTitle}>Anti-Cheating</h3>
              <p className={homeStyles.featureCardDesc}>Randomize question order and answer options. Timed sessions prevent sharing.</p>
            </motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.featureCard}>
              <div className={homeStyles.featureCardIcon}>
                <img src="https://cdn-icons-png.flaticon.com/512/3135/3135715.png" alt="Access" className={homeStyles.featureCardIconImg} />
              </div>
              <h3 className={homeStyles.featureCardTitle}>Access Anywhere</h3>
              <p className={homeStyles.featureCardDesc}>Students take exams from any device — phone, tablet, or computer. No app install needed.</p>
            </motion.div>
          </div>
        </motion.div>
      </section>

      {/* How It Works */}
      <section className={homeStyles.howItWorks}>
        <motion.div initial="hidden" whileInView="visible" viewport={{ once: true, amount: 0.2 }} variants={staggerContainer}>
          <motion.h2 variants={fadeInUp} className={homeStyles.sectionTitle}>How It Works</motion.h2>
          <div className={homeStyles.steps}>
            <motion.div variants={fadeInUp} className={homeStyles.step}>
              <div className={homeStyles.stepNumber}>1</div>
              <h3 className={homeStyles.stepTitle}>Create</h3>
              <p className={homeStyles.stepDesc}>Build your exam with our intuitive question builder. Add questions, set passing scores, and configure time limits.</p>
            </motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.stepConnector}></motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.step}>
              <div className={homeStyles.stepNumber}>2</div>
              <h3 className={homeStyles.stepTitle}>Assign</h3>
              <p className={homeStyles.stepDesc}>Create assignments, set exam windows, and add your students. They'll receive their access automatically.</p>
            </motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.stepConnector}></motion.div>
            <motion.div variants={fadeInUp} className={homeStyles.step}>
              <div className={homeStyles.stepNumber}>3</div>
              <h3 className={homeStyles.stepTitle}>Grade</h3>
              <p className={homeStyles.stepDesc}>Objective questions are graded instantly. Review analytics and export results for your records.</p>
            </motion.div>
          </div>
        </motion.div>
      </section>

      {/* CTA Section */}
      <section className={homeStyles.ctaSection}>
        <motion.div initial="hidden" whileInView="visible" viewport={{ once: true, amount: 0.5 }} variants={staggerContainer}>
          <motion.h2 variants={fadeInUp} className={homeStyles.ctaTitle}>Ready to Transform Your Exams?</motion.h2>
          <motion.p variants={fadeInUp} className={homeStyles.ctaSubtitle}>Join academic institutions using ExamNova to streamline their assessment process.</motion.p>
          <motion.div variants={fadeInUp} className={homeStyles.ctaActions}>
            <Link to={ROUTES.REGISTER} className={homeStyles.btnPrimary}>
              Get Started Free
            </Link>
            <Link to={ROUTES.DEMO} className={homeStyles.btnOutline}>
              Try a Demo Exam
            </Link>
          </motion.div>
        </motion.div>
      </section>

      {/* Footer */}
      <footer className={homeStyles.footer}>
        <p className={homeStyles.footerText}>
          © 2026 ExamNova. Smart examination platform for academic institutions.
        </p>
      </footer>
    </div>
  );
}

function AppContent() {
  const location = useLocation();
  const isExamPage = location.pathname.startsWith('/exam/') || 
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
        <Route path={ROUTES.MY_EXAMS} element={<MyExams />} />
        <Route path={`${ROUTES.EXAM}:examId`} element={<ExamExecutionPage />} />
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
        <Route path={ROUTES.PARENT_DASHBOARD} element={
          <RoleGuard requiredRoles={[UserRole.PARENT]} redirectTo={ROUTES.LOGIN}>
            <ParentDashboard />
          </RoleGuard>
        } />
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