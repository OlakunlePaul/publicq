import { Assignment, AssignmentCreate, AssignmentUpdate } from "../models/assignment";
import { ResponseWithData } from "../models/responseWithData";
import { Response } from "../models/response";
import { GenericOperationStatuses } from "../models/GenericOperationStatuses";
import axios from "../api/axios";
import { PaginatedResponse } from "../models/paginatedResponse";
import { User } from "../models/user";

export const assignmentService = {

  createAssignment: async (assignment: AssignmentCreate): Promise<ResponseWithData<Assignment, GenericOperationStatuses>> => {
    // Implementation for creating an assignment
    const response = await axios.post<ResponseWithData<Assignment, GenericOperationStatuses>>('assignments', assignment);
    return response.data;
  },

  getAssignmentsAsync: async (pageNumber: number, pageSize: number, filterTitle?: string): Promise<ResponseWithData<PaginatedResponse<Assignment>, GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<PaginatedResponse<Assignment>, GenericOperationStatuses>>('assignments',
      { params: { pageNumber, pageSize, filterTitle } });
    return response.data;
  },

  getAvailableAssignments: async (userId: string): Promise<ResponseWithData<Assignment[], GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<Assignment[], GenericOperationStatuses>>(`assignments/available/${userId}`);
    return response.data;
  },

  getAssignmentById: async (id: string): Promise<ResponseWithData<Assignment, GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<Assignment, GenericOperationStatuses>>(`assignments/${id}`);
    return response.data;
  },

  updateAssignment: async (id: string, assignment: AssignmentUpdate): Promise<ResponseWithData<Assignment, GenericOperationStatuses>> => {
    const response = await axios.put<ResponseWithData<Assignment, GenericOperationStatuses>>('assignments', assignment);
    return response.data;
  },

  deleteAssignment: async (id: string): Promise<Response<GenericOperationStatuses>> => {
    const response = await axios.delete<Response<GenericOperationStatuses>>(`assignments/${id}`);
    return response.data;
  },

  getStudents: async (id: string): Promise<ResponseWithData<User[], GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<User[], GenericOperationStatuses>>(`assignments/${id}/exam-takers`);
    return response.data;
  },

  addStudents: async (id: string, studentIds: string[]): Promise<ResponseWithData<Assignment, GenericOperationStatuses>> => {
    const response = await axios.post<ResponseWithData<Assignment, GenericOperationStatuses>>(`assignments/${id}/exam-takers`,
      studentIds);
    return response.data;
  },

  deleteStudents: async (id: string, studentIds: string[]): Promise<ResponseWithData<Assignment, GenericOperationStatuses>> => {
    const response = await axios.delete<ResponseWithData<Assignment, GenericOperationStatuses>>(`assignments/${id}/exam-takers`,
      { data: studentIds });
    return response.data;
  },

  publishAssignment: async (id: string): Promise<Response<GenericOperationStatuses>> => {
    const response = await axios.post<Response<GenericOperationStatuses>>(`assignments/${id}/publish`);
    return response.data;
  },

  getTotalAssignments: async (): Promise<ResponseWithData<number, GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<number, GenericOperationStatuses>>('assignments/total');
    return response.data;
  },

  recordTabSwitch: async (assignmentId: string, studentId: string): Promise<Response<GenericOperationStatuses>> => {
    const response = await axios.post<Response<GenericOperationStatuses>>(`assignments/${assignmentId}/proctoring/tab-switch/${studentId}`);
    return response.data;
  },

  unlockAssignment: async (assignmentId: string, studentId: string): Promise<Response<GenericOperationStatuses>> => {
    const response = await axios.post<Response<GenericOperationStatuses>>(`assignments/${assignmentId}/proctoring/unlock/${studentId}`);
    return response.data;
  },

  getProctoringLogs: async (
    assignmentId: string,
    pageNumber: number = 1,
    pageSize: number = 10,
    studentId?: string
  ): Promise<ResponseWithData<PaginatedResponse<any>, GenericOperationStatuses>> => {
    const response = await axios.get<ResponseWithData<PaginatedResponse<any>, GenericOperationStatuses>>(
      `assignments/${assignmentId}/proctoring-logs`,
      { params: { pageNumber, pageSize, studentId } }
    );
    return response.data;
  }
};