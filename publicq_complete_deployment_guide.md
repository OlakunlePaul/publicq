# PublicQ: Complete Deployment & Usage Guide
## Full Production Setup + Render Deployment + School Management

**Date:** February 16, 2026  
**System:** PublicQ v1.0.4 (MIT Licensed)  
**Purpose:** Production deployment for Nigerian school exam system  

---

## TABLE OF CONTENTS

1. [Local Development Setup](#1-local-development-setup)
2. [Render Production Deployment](#2-render-production-deployment)
3. [Complete User Management Guide](#3-complete-user-management-guide)
4. [Creating Exams (Assessment Modules)](#4-creating-exams-assessment-modules)
5. [Assignment Distribution](#5-assignment-distribution)
6. [Student Exam Taking Process](#6-student-exam-taking-process)
7. [Results & Reports](#7-results--reports)
8. [School-Specific Configuration](#8-school-specific-configuration)
9. [Troubleshooting](#9-troubleshooting)

---

## 1. LOCAL DEVELOPMENT SETUP

### Prerequisites Installation

```bash
# 1. Install .NET 10 SDK
# Download from: https://dotnet.microsoft.com/download/dotnet/10.0

# Verify installation:
dotnet --version
# Should show: 10.0.x

# 2. Install Node.js 20+
# Download from: https://nodejs.org/

# Verify installation:
node --version  # Should show: v20.x.x
npm --version   # Should show: 10.x.x

# 3. Install Git
# Download from: https://git-scm.com/
```

---

### Clone and Build PublicQ

```bash
# 1. Clone repository
git clone https://github.com/MTokarev/publicq.git
cd publicq

# 2. Build Frontend (one-time setup)
cd client
npm install
# This might take 5-10 minutes (installing React 19, TypeScript, etc.)

npm run build
# This creates optimized production build in client/build/

cd ..
# You're back in publicq/ root directory
```

---

### Setup Backend Database

```bash
# Navigate to backend project
cd src/PublicQ.API

# Restore .NET packages
dotnet restore

# Create database and apply migrations
dotnet ef database update --context ApplicationDbContext

# This creates: src/PublicQ.API/db/publicq.db (SQLite database)

# Run the application
dotnet run

# You should see:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: http://localhost:5188
# Application started. Press Ctrl+C to shut down.
```

---

### Access Local Installation

**Open browser:**
- Main app: http://localhost:5188
- API docs: http://localhost:5188/swagger

**Default Login:**
```
Email: admin@publicq.local
Password: admin
```

⚠️ **CRITICAL**: Change this password immediately after first login!

---

### Local File Structure

```
publicq/
├── client/                  # React frontend
│   ├── build/              # Production build (served by backend)
│   ├── public/             # Static assets
│   └── src/                # React source code
├── src/
│   └── PublicQ.API/        # .NET backend
│       ├── db/
│       │   └── publicq.db  # SQLite database
│       ├── static/         # Uploaded files (images, documents)
│       ├── appsettings.json
│       └── Program.cs
├── docs/                    # Documentation
├── Dockerfile
└── docker-compose.yml
```

---

## 2. RENDER PRODUCTION DEPLOYMENT

### Why Render?
- ✅ Free tier available (good for testing)
- ✅ Automatic HTTPS
- ✅ Easy PostgreSQL database
- ✅ Git-based deployment
- ✅ Environment variables management

---

### Step 1: Prepare for Deployment

**A. Switch to PostgreSQL (Required for Render)**

PublicQ uses SQLite by default. For production on Render, we'll use PostgreSQL.

**Edit:** `src/PublicQ.API/PublicQ.API.csproj`

Find:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.0" />
```

Add below it:
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
```

**Edit:** `src/PublicQ.API/appsettings.json`

Change:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=db/publicq.db"
  }
}
```

To:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

(We'll set this via environment variable on Render)

**Edit:** `src/PublicQ.API/Infrastructure/Data/ApplicationDbContext.cs`

Find (around line 25):
```csharp
options.UseSqlite(connectionString);
```

Replace with:
```csharp
options.UseNpgsql(connectionString);
```

**Commit changes:**
```bash
git add .
git commit -m "Prepare for Render deployment with PostgreSQL"
git push origin main
```

---

### Step 2: Create Render Account & Services

**A. Sign up for Render**
- Go to: https://render.com
- Click "Get Started"
- Sign up with GitHub (easiest)
- Authorize Render to access your repositories

---

**B. Create PostgreSQL Database**

1. Click "New +" → "PostgreSQL"
2. Settings:
   - **Name:** `publicq-db`
   - **Database:** `publicq`
   - **User:** `publicq_user` (auto-generated)
   - **Region:** Choose closest to Nigeria (e.g., Frankfurt)
   - **Instance Type:** Free (for testing)
3. Click "Create Database"
4. Wait 2-3 minutes for provisioning
5. **Copy "Internal Database URL"** (you'll need this)
   - Format: `postgresql://publicq_user:PASSWORD@dpg-xxxxx/publicq`

---

**C. Create Web Service (Backend + Frontend)**

1. Click "New +" → "Web Service"
2. Connect your forked `publicq` repository
3. Settings:

**Basic:**
- **Name:** `publicq-app`
- **Region:** Same as database (Frankfurt)
- **Branch:** `main`
- **Root Directory:** Leave empty
- **Runtime:** Docker
- **Instance Type:** Free (for testing, upgrade to Starter $7/month for production)

**Build:**
- **Dockerfile Path:** `./Dockerfile`

**Environment Variables:**
Click "Add Environment Variable" for each:

```bash
# Essential
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# Database (paste your Internal Database URL)
ConnectionStrings__DefaultConnection=postgresql://publicq_user:PASSWORD@dpg-xxxxx/publicq

# Optional: For better performance
ConnectionStrings__Redis=
# (Leave empty for now, add Redis later if needed)
```

4. Click "Create Web Service"
5. Wait 5-10 minutes for first deployment

---

### Step 3: Run Database Migrations

**A. Access Render Shell**

1. Go to your `publicq-app` service on Render
2. Click "Shell" tab (top right)
3. Run migration command:

```bash
cd /app/src/PublicQ.API
dotnet ef database update --context ApplicationDbContext
```

**If you get "dotnet ef not found":**
```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:/root/.dotnet/tools"
dotnet ef database update --context ApplicationDbContext
```

This creates all tables in your PostgreSQL database.

---

### Step 4: Access Your Production App

**Your app URL:**
- Render provides: `https://publicq-app.onrender.com`
- Or custom domain: `https://yourschool.com` (configure in Render settings)

**Default Login:**
```
Email: admin@publicq.local
Password: admin
```

⚠️ **Change immediately after first login!**

---

### Step 5: Production Checklist

After deployment, verify:

- [ ] App loads at Render URL
- [ ] Can login with admin credentials
- [ ] Database persists (create test user, logout, login - user should exist)
- [ ] File uploads work (try uploading an image)
- [ ] HTTPS works (Render provides automatic SSL)

---

## 3. COMPLETE USER MANAGEMENT GUIDE

### Understanding PublicQ Roles

**7 Roles (in order of permissions):**

1. **Exam Taker** - Students (take exams, view own results)
2. **Analyst** - View reports and analytics
3. **Contributor** - Create exams and questions
4. **Manager** - User management + assignments
5. **Moderator** - All of above + groups
6. **Administrator** - Full system access

For a school:
- **Admin** = You (full control)
- **Manager** = School principal/admin
- **Contributor** = Teachers (create exams)
- **Exam Taker** = Students

---

### A. Creating Admin User (First Time)

**Login as default admin:**
```
URL: https://publicq-app.onrender.com
Email: admin@publicq.local
Password: admin
```

**Change password:**
1. Click user icon (top right)
2. Settings → Change Password
3. Set strong password (min 8 chars, 1 uppercase, 1 number)

---

### B. Creating Teachers (Contributors)

**Path:** Users → Create User

**Form:**
```
Email: teacher1@infinixschool.com
First Name: Funke
Last Name: Adebayo
Date of Birth: 15/05/1985
Role: Contributor (or Manager if they handle admin tasks)
Password: TempPass123!
Confirm Password: TempPass123!
```

**Permissions (Contributor role gives):**
- ✅ Create assessment modules (exams)
- ✅ Create questions
- ✅ Upload files (images for questions)
- ✅ Use AI assistant for content creation
- ❌ Cannot create users
- ❌ Cannot assign exams

Click "Create User"

**Send credentials to teacher:**
```
Hi Mrs. Adebayo,

Your exam creation account:
URL: https://publicq-app.onrender.com
Email: teacher1@infinixschool.com
Password: TempPass123!

Please login and change your password immediately.

Best regards
```

---

### C. Creating Students (Exam Takers) - BULK IMPORT

**Individual Method (slow):**

Path: Users → Create User
```
Email: student1@infinixschool.com
First Name: Chioma
Last Name: Okonkwo
Date of Birth: 10/03/2010
Role: Exam Taker
Password: Student123!
```

**Bulk Import Method (FASTER):**

**Step 1: Download CSV Template**

Path: Users → Bulk Import → Download CSV Template

This downloads: `user-import-template.csv`

**Step 2: Fill Template**

Open in Excel/Google Sheets:

```csv
email,firstName,lastName,dateOfBirth,role
student1@infinixschool.com,Chioma,Okonkwo,2010-03-10,ExamTaker
student2@infinixschool.com,Tunde,Bakare,2009-08-15,ExamTaker
student3@infinixschool.com,Amina,Ibrahim,2010-01-22,ExamTaker
student4@infinixschool.com,Emeka,Nwosu,2009-11-05,ExamTaker
```

**Rules:**
- ✅ Date format: YYYY-MM-DD (e.g., 2010-03-10)
- ✅ Role: ExamTaker (exactly this spelling)
- ✅ Valid email addresses
- ✅ Max 500 users per file

**Step 3: Upload CSV**

Path: Users → Bulk Import → Upload CSV

1. Select your filled CSV file
2. **Optional:** Select an assignment to auto-assign students
3. Click "Import Users"
4. System creates all accounts with auto-generated passwords

**Step 4: Get Student Credentials**

After import:
1. Go to Users → List
2. Search for imported students
3. Click on each student
4. View → "Reset Password" → System generates new password
5. Copy credentials to give to students/parents

**OR use default password method:**

Edit all imported users to set same temp password:
- Teachers can send: `TempPass123!` to all students
- Students login and change password

---

### D. Managing User Roles

**To change a user's role:**

1. Users → List
2. Search for user (name or email)
3. Click user row
4. Edit button
5. Change "Role" dropdown
6. Save

**Example:** Promote teacher to Manager
- Find: teacher1@infinixschool.com
- Change Role: Contributor → Manager
- Save

Now they can create users and assignments too.

---

### E. Password Reset (Students Forgot Password)

**Admin process:**

1. Users → List
2. Find student
3. Click student
4. "Reset Password" button
5. System generates new password
6. Copy and send to student/parent via WhatsApp

**Self-service reset:** PublicQ doesn't have this yet (requires email configuration)

---

### F. Disabling/Deleting Users

**To disable student who left school:**

1. Users → List
2. Find student
3. Click → Edit
4. Toggle "Active" to OFF
5. Save

User can't login but data is preserved.

**To permanently delete:**

1. Find user
2. Delete button (⚠️ cannot be undone!)
3. Confirm

---

## 4. CREATING EXAMS (ASSESSMENT MODULES)

### Understanding PublicQ Structure

```
Assessment Module (Exam)
  ├── Version 1
  │   ├── Question 1 (Multiple Choice)
  │   ├── Question 2 (True/False)
  │   └── Question 3 (Essay)
  └── Version 2 (updated questions)
```

**Why versions?**
- Update exams without losing old results
- Create different difficulty levels
- Track improvements over time

---

### A. Creating Your First Exam

**Login as Contributor (Teacher)**

Path: Modules → Create Module

**Form:**

```
Title: Mathematics Mid-Term Exam - JSS 1
Description: Covers topics 1-5: Numbers, Fractions, Decimals, 
             Basic Algebra, Geometry

Settings:
- Duration: 60 minutes
- Passing Score: 50%
- Allow Review: Yes (students can review answers before submit)
- Randomize Questions: Yes (prevent cheating)
- Randomize Answers: Yes (for multiple choice)

Status: Draft (not visible to students yet)
```

Click "Create Module"

---

### B. Adding Questions

**Path:** Modules → Your Module → Add Question

#### **Question Type 1: Multiple Choice**

```
Question Text: What is 25 + 17?

Question Type: Multiple Choice

Answers:
1. Answer: 32
   ✓ Correct
   Points: 1

2. Answer: 42
   ☐ Correct
   Points: 0

3. Answer: 52
   ☐ Correct
   Points: 0

4. Answer: 27
   ☐ Correct
   Points: 0

Explanation (shown after submission):
25 + 17 = 42. Add the ones place (5+7=12), 
carry 1 to tens place (2+1+1=4).

Attach File (optional): Upload diagram/image
```

Click "Add Question"

---

#### **Question Type 2: True/False**

```
Question Text: The capital of Nigeria is Lagos.

Question Type: True/False

Answers:
1. Answer: True
   ☐ Correct
   Points: 0

2. Answer: False
   ✓ Correct
   Points: 1

Explanation: The capital of Nigeria is Abuja, not Lagos. 
Lagos is the commercial capital.
```

---

#### **Question Type 3: Essay (Open-Ended)**

```
Question Text: Explain the water cycle in 3-4 sentences.

Question Type: Open-Ended

Expected Answer/Rubric:
Should mention: evaporation, condensation, precipitation, collection.
Award full points if all 4 stages mentioned.

Points: 5 (manually graded)

Explanation: N/A (teacher grades manually)
```

**Note:** Essay questions require manual grading after exam.

---

### C. Adding Images to Questions

**When to use:**
- Math diagrams (geometry)
- Science illustrations
- Reading comprehension passages
- Charts/graphs

**How to add:**

1. While creating question
2. Scroll to "Attach File" section
3. Click "Choose File"
4. Select image (JPEG, PNG, max 5MB)
5. Upload
6. Image appears in question preview

**Pro tip:** Use simple, clear images. Avoid large files (slow loading on Nigerian mobile networks).

---

### D. Publishing the Exam

**After adding all questions:**

1. Modules → Your Module
2. Review all questions (Preview button)
3. Click "Publish" (changes status from Draft to Published)

**⚠️ Important:**
- Only published modules can be assigned to students
- Published modules are locked (can't edit)
- To make changes: Create new version

---

### E. Creating Exam Versions

**Scenario:** You want to improve the exam after first use.

**Steps:**

1. Modules → Find your published module
2. Click "Create New Version"
3. System copies all questions to Version 2
4. Status: Draft
5. Edit questions as needed
6. Publish Version 2

**Result:**
- Old assignments use Version 1 (historical results preserved)
- New assignments can use Version 2

---

### F. Using AI Assistant for Question Creation

**PublicQ has built-in AI (AI Monkey 🐵)**

**To use:**

1. Modules → Your Module
2. Click "AI Chat" button (bottom right)
3. Type prompt:

```
Create 10 multiple choice questions about fractions 
for JSS 1 students. Include:
- Adding fractions
- Subtracting fractions  
- Equivalent fractions
- Simplifying fractions

Make questions appropriate for 11-12 year olds.
```

4. AI generates questions
5. Review and add to your module
6. Edit as needed

**Pro tip:** Be specific in prompts (grade level, topics, difficulty).

---

## 5. ASSIGNMENT DISTRIBUTION

### Understanding Assignment Flow

```
1. Admin creates GROUP (collection of modules/exams)
2. Admin creates ASSIGNMENT from group
3. Admin adds individual STUDENTS to assignment
4. Students see assignment on their dashboard
5. Students take exams
6. Results are recorded
```

---

### A. Creating Groups (Exam Collections)

**Path:** Groups → Create Group

**Example for Mid-Term Exams:**

```
Group Name: JSS 1 Mid-Term Exams - 2nd Term 2026

Description: Complete mid-term assessment for JSS 1 students

Modules in this Group:
☑ Mathematics Mid-Term Exam - JSS 1
☑ English Language Mid-Term Exam - JSS 1
☑ Basic Science Mid-Term Exam - JSS 1
☐ History Mid-Term Exam - JSS 1 (not ready yet)

Settings:
- Module Order: Sequential (students must complete in order)
- Retake Allowed: No
```

Click "Create Group"

**Why groups?**
- Students take multiple related exams
- Consistent exam sets
- Easier assignment management

---

### B. Creating Assignment

**Path:** Assignments → Create Assignment

```
Assignment Title: JSS 1A - 2nd Term Mid-Term Exams

Group: JSS 1 Mid-Term Exams - 2nd Term 2026
       (select from dropdown)

Schedule:
- Start Date: 20 Feb 2026, 09:00 AM
- End Date: 22 Feb 2026, 05:00 PM
- Duration per Module: 60 minutes
  (students have 60 min once they start each exam)

Settings:
☑ Show Results Immediately (students see score after submission)
☑ Allow Review (students can review answers before final submit)
☐ Allow Retake (one attempt only)

Status: Draft (not visible to students yet)
```

Click "Create Assignment"

---

### C. Adding Students to Assignment

**Path:** Assignments → Your Assignment → Add Students

**Method 1: Individual Selection**

```
Search students:
[Chioma Okonkwo_______________] [Search]

Results:
☐ Chioma Okonkwo (student1@infinixschool.com) - JSS 1A
☐ Tunde Bakare (student2@infinixschool.com) - JSS 1A
☐ Amina Ibrahim (student3@infinixschool.com) - JSS 1B

Select all JSS 1A students → Add to Assignment
```

**Method 2: Bulk Import (during user creation)**

When importing students via CSV:
1. Upload CSV
2. Select Assignment: "JSS 1A - 2nd Term Mid-Term Exams"
3. Import → Students are auto-assigned

---

### D. Publishing Assignment

**Before publishing:**
- Verify all students added
- Check start/end dates
- Test with one student (optional)

**To publish:**

1. Assignments → Your Assignment
2. Change Status: Draft → Published
3. Save

**What happens:**
- Students see assignment on their dashboard
- Email notifications sent (if configured)
- Assignment becomes active at start date

---

### E. Managing Active Assignments

**To view who completed:**

1. Assignments → Your Assignment
2. "Submissions" tab

```
Student Progress:
✓ Chioma Okonkwo - Completed (Score: 85%)
⏳ Tunde Bakare - In Progress (2/3 modules done)
☐ Amina Ibrahim - Not Started
```

**To extend deadline for individual student:**

1. Find student in submissions
2. Click "Extend Deadline"
3. Add hours/days
4. Save

Useful if student was sick or had technical issues.

---

## 6. STUDENT EXAM TAKING PROCESS

### A. Student Login

**Student receives:**
```
WhatsApp message:
"Your exam is ready!
URL: https://publicq-app.onrender.com
Email: student1@infinixschool.com
Password: TempPass123!

Exams available: Feb 20-22
Good luck!"
```

**Student logs in:**
1. Go to URL
2. Enter email + password
3. Click Login
4. First login: Forced to change password

---

### B. Student Dashboard

**After login, student sees:**

```
📚 My Assignments

┌──────────────────────────────────────┐
│ JSS 1A - 2nd Term Mid-Term Exams     │
│ Status: Available                    │
│ Due: 22 Feb 2026, 5:00 PM           │
│                                      │
│ Modules (3):                         │
│ ☐ Mathematics (60 min)               │
│ ☐ English Language (60 min)          │
│ ☐ Basic Science (60 min)             │
│                                      │
│ [Start Assignment]                   │
└──────────────────────────────────────┘
```

---

### C. Taking the Exam

**Student clicks "Start Assignment":**

```
⚠️ Important Instructions:

- You have 60 minutes to complete this exam
- Timer starts when you click "Begin Exam"
- Your answers are auto-saved every 30 seconds
- You can navigate between questions
- Click "Submit" when finished
- Exam auto-submits when time expires

☑ I have read and understood the instructions

[Cancel]  [Begin Exam]
```

**Student clicks "Begin Exam":**

```
Mathematics Mid-Term Exam - JSS 1        ⏱️ 59:32

Question 1 of 20

What is 25 + 17?

○ 32
○ 42
○ 52
○ 27

[Previous] [Flag for Review] [Next]

Progress: ■■■□□□□□□□□□□□□□□□□□ (3/20)

[Save & Exit] [Submit Exam]
```

**Key Features:**
- ⏱️ **Timer**: Counts down, visible always
- **Auto-save**: Answers saved automatically
- **Navigation**: Previous/Next to move between questions
- **Flag for Review**: Mark difficult questions
- **Progress bar**: Visual progress

---

### D. Submitting Exam

**Student completes all questions:**

```
📋 Review Your Answers

You've answered 20/20 questions.

Questions:
✓ Q1: Answered
✓ Q2: Answered
⚠️ Q5: Flagged for review
✓ Q3-20: Answered

Time Remaining: 12:45

[Review Flagged] [Submit Exam]
```

**Student clicks "Submit Exam":**

```
⚠️ Confirm Submission

Are you sure you want to submit?

- You've answered 20/20 questions
- 1 question flagged for review
- Time remaining: 12:45
- You cannot change answers after submission

[Cancel] [Yes, Submit]
```

---

### E. Viewing Results

**After submission (if immediate results enabled):**

```
🎉 Exam Completed!

Mathematics Mid-Term Exam - JSS 1

Your Score: 17/20 (85%)
Status: ✅ PASSED (Passing score: 50%)

Time Taken: 47 minutes 15 seconds

Question Breakdown:
✓ Correct: 17
✗ Incorrect: 3
⏭️ Skipped: 0

[View Detailed Results] [Download Certificate]

Next Module: English Language
[Start Next Module]
```

**Detailed Results:**

```
Question 1: What is 25 + 17?
Your Answer: 42 ✓
Correct Answer: 42
Points: 1/1

Explanation: 25 + 17 = 42. Add ones place (5+7=12), 
carry 1 to tens (2+1+1=4).

---

Question 5: Solve: 3x + 5 = 14
Your Answer: x = 4 ✗
Correct Answer: x = 3
Points: 0/1

Explanation: 3x = 14-5 = 9, so x = 9/3 = 3
```

---

## 7. RESULTS & REPORTS

### A. Teacher Viewing Individual Results

**Path:** Assignments → Your Assignment → Submissions

**Click on student:**

```
Chioma Okonkwo - Mathematics Mid-Term Exam

Overall Score: 17/20 (85%)
Time Taken: 47m 15s
Submitted: 20 Feb 2026, 10:47 AM

Question-by-Question:
Q1: What is 25 + 17? ✓ (1/1 pts)
Q2: Simplify: 12/16 ✓ (1/1 pts)
Q3: Find x: 2x = 10 ✓ (1/1 pts)
Q4: True/False: 5 is prime ✓ (1/1 pts)
Q5: Solve: 3x+5=14 ✗ (0/1 pts) - Incorrect, correct answer: x=3

[Download PDF Report] [Email to Parent]
```

---

### B. Class Performance Report

**Path:** Reports → Class Performance

**Select:**
- Assignment: JSS 1A Mid-Term Exams
- Module: Mathematics
- Generate Report

**Output:**

```
📊 Class Performance Report

Assignment: JSS 1A - 2nd Term Mid-Term Exams
Module: Mathematics Mid-Term Exam - JSS 1
Date Range: 20-22 Feb 2026

Summary:
- Total Students: 32
- Completed: 30 (94%)
- In Progress: 1 (3%)
- Not Started: 1 (3%)

Average Score: 74.2%
Median Score: 76%
Highest Score: 95% (Tunde Bakare)
Lowest Score: 45% (Amina Ibrahim)

Pass Rate: 28/30 (93%)
Fail Rate: 2/30 (7%)

Score Distribution:
90-100%: ████████ 8 students
80-89%:  ██████ 6 students
70-79%:  ████████████ 12 students
60-69%:  ██ 2 students
50-59%:  ░ 0 students
0-49%:   ██ 2 students

Question Analysis:
Q1 (Addition): 95% correct
Q2 (Fractions): 87% correct
Q5 (Algebra): 45% correct ⚠️ (Needs review)
Q12 (Geometry): 62% correct

[Export to Excel] [Download PDF] [Print]
```

---

### C. Individual Student Report (for Parents)

**Path:** Reports → Student Report → Select Student

```
📄 Student Performance Report

Student: Chioma Okonkwo
Class: JSS 1A
Report Period: 2nd Term Mid-Term Exams

Overall Performance:
Mathematics: 85% ✅ PASSED
English: 78% ✅ PASSED
Science: 92% ✅ PASSED

Average: 85% (Grade: A)
Class Rank: 5/32

Strengths:
✓ Science (Top 10%)
✓ Mathematics Problem Solving
✓ Reading Comprehension

Areas for Improvement:
⚠️ Algebra (45% on related questions)
⚠️ Grammar & Punctuation

Teacher Comments:
"Chioma shows excellent understanding of core concepts. 
Recommend additional practice in algebra to strengthen 
weak areas."

[Download PDF] [Email to Parent]
```

---

### D. Exporting Data

**Path:** Reports → Export Data

**Options:**

```
Export Format:
● Excel (.xlsx)
○ CSV (.csv)
○ PDF Report

Data to Export:
☑ Student scores
☑ Question-by-question breakdown
☑ Time taken per question
☐ Raw answer data

Filters:
Assignment: All / Specific
Date Range: 01 Feb - 28 Feb 2026
Students: All / Class / Individual

[Generate Export]
```

**Use cases:**
- Import into school management system
- Parent progress reports
- Ministry of Education reporting
- Academic records

---

## 8. SCHOOL-SPECIFIC CONFIGURATION

### A. Branding the System

**Path:** Admin Panel → Settings → Branding (if available)

**OR customize via:**

1. **School Name in Emails:**
   - Edit email templates
   - Replace "PublicQ" with "Infinix Private Academy"

2. **Logo Upload:**
   - Settings → Upload School Logo
   - Use in PDF certificates

3. **Custom Domain:**
   - Render Settings → Custom Domains
   - Add: exams.infinixschool.com
   - Configure DNS at your domain provider

---

### B. Email Configuration (Notifications)

**Path:** Admin Panel → Settings → Email

```
SMTP Configuration:
Provider: Gmail SMTP
Host: smtp.gmail.com
Port: 587
Username: exams@infinixschool.com
Password: [App Password]
From Name: Infinix Private Academy Exams
From Email: exams@infinixschool.com

Test Email: [Send Test]

Notifications Enabled:
☑ Assignment Created
☑ Assignment Due Soon (24h before)
☑ Exam Completed
☑ Results Available
☐ Password Reset
```

**Gmail App Password:**
1. Go to Google Account → Security
2. 2-Step Verification → App Passwords
3. Generate password for "Mail"
4. Use this in PublicQ settings

---

### C. Exam Scheduling (Nigerian School Calendar)

**Common schedule:**

```
1st Term:
- Resumption: September
- Mid-Term: October
- Final Exams: December

2nd Term:
- Resumption: January
- Mid-Term: February ← You are here
- Final Exams: April

3rd Term:
- Resumption: May
- Mid-Term: June
- Final Exams: July
```

**Schedule assignments accordingly:**

```
Assignment: JSS 1 - 2nd Term Mid-Term
Start: 20 Feb 2026 (Monday)
End: 22 Feb 2026 (Wednesday)
Duration: 60 min per exam
```

---

### D. Offline/Low Internet Considerations

**PublicQ requires internet, but you can optimize:**

1. **Computer Lab Setup:**
   - Stable WiFi or wired internet
   - Test before exam day
   - Have backup dongles (mobile hotspot)

2. **Auto-Save Feature:**
   - Answers saved every 30 seconds
   - Students can resume if disconnected
   - Extend deadline if school-wide outage

3. **Printable Results:**
   - Generate PDFs for students without internet at home
   - Parents collect printed results

4. **Mobile Data Usage:**
   - PublicQ is lightweight (text-based)
   - Minimal data usage (~1-2MB per exam)
   - Images increase data usage

---

### E. Anti-Cheating Measures

**Built-in:**
- ✅ Randomize question order (each student different)
- ✅ Randomize answer order
- ✅ Timer (pressure reduces cheating time)
- ✅ One attempt only
- ✅ Tab switching detection (warns if student leaves page)

**Additional (manual):**
- Conduct exams in computer lab (teacher supervision)
- Space students apart
- Monitor during exam time
- Disable phone usage policy

---

## 9. TROUBLESHOOTING

### Common Issues & Solutions

#### **Issue 1: Students Can't Login**

**Symptoms:**
- "Invalid email or password"
- Account locked

**Solutions:**
```
1. Check email spelling (copy-paste, don't type)
2. Password is case-sensitive
3. If forgotten: Admin resets password
   - Users → Find Student → Reset Password
4. If account locked:
   - Users → Find Student → Edit → Active: ON
```

---

#### **Issue 2: Assignment Not Showing**

**Student says:** "I don't see the exam"

**Check:**
```
1. Is assignment Published? (not Draft)
2. Is student added to assignment?
   - Assignments → Your Assignment → Submissions
   - Check if student name appears
3. Is current date within Start/End dates?
4. Has student already completed it?
   - Check submission status
```

**Fix:**
- Add student: Assignments → Add Students → Select & Add
- Extend dates if needed

---

#### **Issue 3: Timer Expired, Student Needs More Time**

**Scenario:** Student had power outage during exam

**Solution:**
```
1. Assignments → Find Assignment
2. Submissions → Find Student
3. "Extend Deadline" or "Allow Retake"
4. Add time (e.g., +60 minutes)
5. Save
6. Student can resume
```

---

#### **Issue 4: File Upload Fails**

**Error:** "File too large" or "Upload failed"

**Solutions:**
```
1. Check file size (max 5MB)
2. Use JPEG instead of PNG (smaller)
3. Compress images:
   - TinyPNG.com
   - Windows: Paint → Resize → Save
4. Supported formats: JPEG, PNG, PDF, DOCX
5. Check internet connection
```

---

#### **Issue 5: Render App Not Loading**

**Error:** "Application Error" or "502 Bad Gateway"

**Check:**
```
1. Render Dashboard → Your Service
2. Check "Logs" tab for errors
3. Common causes:
   - Database connection failed (check ConnectionStrings)
   - Out of memory (upgrade from Free to Starter)
   - Deployment failed (check Build logs)

Solutions:
- Restart service: Settings → Manual Deploy
- Check environment variables
- Upgrade instance type
```

---

#### **Issue 6: Database Migration Failed**

**Error:** "Unable to connect to database"

**Solution:**
```
1. Check environment variable:
   ConnectionStrings__DefaultConnection=postgresql://...

2. Verify database is running:
   Render → Your Database → Status: Available

3. Re-run migration:
   Shell → cd /app/src/PublicQ.API
   → dotnet ef database update

4. If still fails:
   - Check Internal vs External Database URL
   - Use Internal URL for backend
```

---

#### **Issue 7: Students Getting Same Questions**

**Want randomization:**

**Check Module Settings:**
```
1. Modules → Your Module → Edit
2. Settings:
   ☑ Randomize Questions: ON
   ☑ Randomize Answers: ON
3. Save
4. Create new assignment (old assignments keep old settings)
```

**Note:** Questions from same pool, but different order

---

#### **Issue 8: Performance Slow with Many Students**

**App is slow when 100+ students taking exam**

**Solutions:**
```
1. Upgrade Render Instance:
   - Free → Starter ($7/month)
   - Gives more RAM and CPU

2. Enable Redis Cache:
   - Add Redis service on Render
   - Add environment variable:
     ConnectionStrings__Redis=redis://...

3. Schedule exams in batches:
   - JSS 1A: Monday 9-11am
   - JSS 1B: Monday 12-2pm
   - JSS 2A: Tuesday 9-11am
```

---

### Getting Help

**PublicQ Support:**
- GitHub Issues: https://github.com/MTokarev/publicq/issues
- Email: publicq-app@outlook.com
- Documentation: https://github.com/MTokarev/publicq/tree/main/docs

**Render Support:**
- Docs: https://render.com/docs
- Status: https://status.render.com
- Support: help@render.com

---

## APPENDIX: QUICK REFERENCE

### Default Credentials
```
URL: https://publicq-app.onrender.com
Admin Email: admin@publicq.local
Admin Password: admin (CHANGE IMMEDIATELY!)
```

### Important Paths
```
Users: /users
Modules: /modules
Groups: /groups
Assignments: /assignments
Reports: /reports
Settings: /settings
```

### Role Permissions Summary
| Action | Exam Taker | Contributor | Manager | Admin |
|--------|-----------|-------------|---------|-------|
| Take Exams | ✅ | ✅ | ✅ | ✅ |
| Create Exams | ❌ | ✅ | ✅ | ✅ |
| Create Users | ❌ | ❌ | ✅ | ✅ |
| Create Assignments | ❌ | ❌ | ✅ | ✅ |
| View All Reports | ❌ | ❌ | ✅ | ✅ |
| System Settings | ❌ | ❌ | ❌ | ✅ |

### Render Deployment Checklist
- [ ] Fork PublicQ repo
- [ ] Create PostgreSQL database on Render
- [ ] Create Web Service on Render
- [ ] Set environment variables
- [ ] Deploy
- [ ] Run database migrations
- [ ] Change default admin password
- [ ] Test with sample exam
- [ ] Add real users
- [ ] Configure email (optional)

---

**END OF GUIDE**

**Next Steps:**
1. Deploy to Render following Section 2
2. Create admin and teacher accounts (Section 3)
3. Create first exam (Section 4)
4. Assign to students (Section 5)
5. Monitor results (Section 7)

**Estimated Setup Time:** 2-3 hours for complete deployment and first exam.

Good luck with your school's exam system! 🎓
