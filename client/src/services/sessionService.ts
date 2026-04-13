import axios from "../api/axios";
import { GenericOperationStatuses } from "../models/GenericOperationStatuses";
import { GroupMemberStateWithUserProgress } from "../models/group-member-state-with-user-progress";
import { StudentModuleVersion } from "../models/student-module-version";
import { ResponseWithData } from "../models/responseWithData";
import { Response } from "../models/response";
import { ModuleProgress } from "../models/module-progress";
import { GroupState } from "../models/group-state";
import { QuestionResponseOperation } from "../models/question-response-operation";

export const sessionService = {
  getGroupState: async (studentId: string, studentAssignmentId: string): Promise<ResponseWithData<GroupState, GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<GroupState, GenericOperationStatuses>>(`sessions/${studentId}/assignment/${studentAssignmentId}/group/state`);
    return response.data;
  },

  getGroupMemberStates: async (studentId: string, studentAssignmentId: string, groupId: string): Promise<ResponseWithData<GroupMemberStateWithUserProgress[], GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<GroupMemberStateWithUserProgress[], GenericOperationStatuses>>(`sessions/${studentId}/assignment/${studentAssignmentId}/group/${groupId}/members`);
    return response.data;
  },

  getModuleVersionForStudent: async (studentId: string, assignmentId: string, assessmentModuleVersionId: string): Promise<ResponseWithData<StudentModuleVersion, GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<StudentModuleVersion, GenericOperationStatuses>>(`sessions/${studentId}/assignment/${assignmentId}/module/version/${assessmentModuleVersionId}`);
    return response.data;
  },

  getModuleProgress: async (studentId: string, assignmentId: string, assessmentModuleId: string): Promise<ResponseWithData<ModuleProgress, GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<ModuleProgress, GenericOperationStatuses>>(`sessions/${studentId}/assignment/${assignmentId}/module/${assessmentModuleId}/progress`);
    return response.data;
  },

  createModuleProgress: async (studentId: string, assignmentId: string, assessmentModuleId: string): Promise<ResponseWithData<ModuleProgress, GenericOperationStatuses>> => {
    const response = await axios.post<ResponseWithData<ModuleProgress, GenericOperationStatuses>>(`sessions/${studentId}/assignment/${assignmentId}/module/${assessmentModuleId}/progress`);
    return response.data;
  },

  submitAnswer: async (userProgressId: string, questionResponse: QuestionResponseOperation): Promise<Response<GenericOperationStatuses>> => {
    const response = await axios.post<Response<GenericOperationStatuses>>(`sessions/progress/${userProgressId}/answer`, questionResponse);
    return response.data;
  },

  completeModule: async (userProgressId: string): Promise<Response<GenericOperationStatuses>> => {
    const response = await axios.post<Response<GenericOperationStatuses>>(`sessions/progress/${userProgressId}/complete`);
    return response.data;
  },

  updateQuestionResponseMark: async (responseId: string, isCorrect: boolean): Promise<Response<GenericOperationStatuses>> => {
    const response = await axios.patch<Response<GenericOperationStatuses>>(`sessions/responses/${responseId}/mark?isCorrect=${isCorrect}`);
    return response.data;
  },

  extendTime: async (userProgressId: string, minutes: number): Promise<Response<GenericOperationStatuses>> => {
    const response = await axios.post<Response<GenericOperationStatuses>>(`sessions/progress/${userProgressId}/extend-time?minutes=${minutes}`);
    return response.data;
  },

  bulkExtendTime: async (sessionId: string, termId: string, classId: string, subjectId: string, minutes: number): Promise<Response<GenericOperationStatuses>> => {
    const response = await axios.post<Response<GenericOperationStatuses>>('sessions/bulk-extend-time', {
      sessionId,
      termId,
      classId,
      subjectId,
      minutes
    });
    return response.data;
  }
}