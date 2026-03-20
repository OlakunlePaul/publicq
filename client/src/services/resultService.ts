import api from '../api/axios';
import { Response as AppResponse } from '../models/response';

export enum ModerationStatus {
  Draft = 0,
  Moderated = 1,
  Approved = 2,
  Published = 3
}

export interface SubjectScore {
  id: string;
  subjectName: string;
  testScore: number;
  examScore: number;
  totalScore: number;
  grade: string;
  subjectRemark: string;
}

export interface StudentAssessment {
  id: string;
  studentId: string;
  studentName: string;
  admissionNumber: string;
  sessionName: string;
  termName: string;
  className: string;
  status: ModerationStatus;
  isLockedForParents: boolean;
  createdAt: string;
  totalMarksObtained?: number;
  totalMarksObtainable?: number;
  averageScore?: number;
  positionInClass?: number;
  numberInClass?: number;
  overallGrade?: string;
  timesSchoolOpened?: number;
  timesPresent?: number;
  timesAbsent?: number;
  classTeacherComment?: string;
  headTeacherComment?: string;
  subjectScores: SubjectScore[];
}

export interface ResultUploadResponse {
  totalProcessed: number;
  successCount: number;
  failureCount: number;
  errors: string[];
}

export const resultService = {
  uploadCsv: async (file: File, sessionId: string, termId: string, classLevelId: string): Promise<ResultUploadResponse> => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('sessionId', sessionId);
    formData.append('termId', termId);
    formData.append('classLevelId', classLevelId);

    const r = await api.post<ResultUploadResponse>('results/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return r.data;
  },

  getReportCard: async (assessmentId: string): Promise<StudentAssessment> => {
    const r = await api.get<StudentAssessment>(`results/report-card/${assessmentId}`);
    return r.data;
  },

  getClassResults: async (sessionId: string, termId: string, classLevelId: string): Promise<StudentAssessment[]> => {
    const r = await api.get<StudentAssessment[]>('results/class', {
      params: { sessionId, termId, classLevelId }
    });
    return r.data;
  },

  getParentChildrenResults: async (): Promise<StudentAssessment[]> => {
    const r = await api.get<StudentAssessment[]>('results/parent/children');
    return r.data;
  },

  updateStatus: async (assessmentId: string, status: ModerationStatus): Promise<void> => {
    await api.patch(`results/${assessmentId}/status`, status, {
        headers: {
            'Content-Type': 'application/json'
        }
    });
  },

  // Added missing methods for ResultManagement.tsx compatibility
  saveBulkScores: async (payload: any): Promise<{ isSuccess: boolean, message?: string }> => {
    await api.post('results/bulk-scores', payload);
    return { isSuccess: true };
  },

  calculateClassResults: async (sessionId: string, termId: string, classLevelId: string): Promise<{ isSuccess: boolean, message?: string }> => {
    await api.post('results/calculate', { sessionId, termId, classLevelId });
    return { isSuccess: true };
  },

  toggleAssessmentLock: async (assessmentId: string, isLocked: boolean): Promise<{ isSuccess: boolean, message?: string }> => {
    await api.patch(`results/${assessmentId}/lock`, { isLocked });
    return { isSuccess: true };
  },

  batchUpdateClassStatus: async (sessionId: string, termId: string, classLevelId: string, currentStatus: number, newStatus: number): Promise<{ isSuccess: boolean, message?: string }> => {
    await api.patch('results/batch-status', { sessionId, termId, classLevelId, currentStatus, newStatus });
    return { isSuccess: true };
  },

  syncOnlineScores: async (sessionId: string, termId: string, classLevelId: string): Promise<{ isSuccess: boolean, message?: string }> => {
    const r = await api.post<AppResponse>('results/sync-online-scores', { sessionId, termId, classLevelId });
    return { isSuccess: true, message: r.data?.message };
  }
};
