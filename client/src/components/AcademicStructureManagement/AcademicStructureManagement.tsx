import React, { useState } from 'react';
import commonStyles from '../Admin/AdminCommon.module.css';
import SubjectManagement from './SubjectManagement';
import ClassManagement from './ClassManagement';
import SessionManagement from './SessionManagement';
import TermManagement from './TermManagement';
import GradingManagement from './GradingManagement';

const AcademicStructureManagement: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'sessions' | 'terms' | 'classes' | 'subjects' | 'grading'>('subjects');

  return (
    <div className={commonStyles.container}>
      <h2 className={commonStyles.title}>Academic Structure</h2>
      <p className={commonStyles.description}>
        Manage academic sessions, terms, classes, and subjects.
      </p>

      <div className={commonStyles.tabs}>
        <button 
          className={`${commonStyles.tab} ${activeTab === 'subjects' ? commonStyles.activeTab : ''}`}
          onClick={() => setActiveTab('subjects')}
        >
          Subjects
        </button>
        <button 
          className={`${commonStyles.tab} ${activeTab === 'sessions' ? commonStyles.activeTab : ''}`}
          onClick={() => setActiveTab('sessions')}
        >
          Sessions
        </button>
        <button 
          className={`${commonStyles.tab} ${activeTab === 'terms' ? commonStyles.activeTab : ''}`}
          onClick={() => setActiveTab('terms')}
        >
          Terms
        </button>
        <button 
          className={`${commonStyles.tab} ${activeTab === 'classes' ? commonStyles.activeTab : ''}`}
          onClick={() => setActiveTab('classes')}
        >
          Classes
        </button>
        <button 
          className={`${commonStyles.tab} ${activeTab === 'grading' ? commonStyles.activeTab : ''}`}
          onClick={() => setActiveTab('grading')}
        >
          Grading
        </button>
      </div>

      <div className={commonStyles.tabContent}>
        {activeTab === 'subjects' && <SubjectManagement />}
        {activeTab === 'sessions' && <SessionManagement />}
        {activeTab === 'terms' && <TermManagement />}
        {activeTab === 'classes' && <ClassManagement />}
        {activeTab === 'grading' && <GradingManagement />}
      </div>
    </div>
  );
};

export default AcademicStructureManagement;
