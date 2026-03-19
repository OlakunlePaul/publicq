import { GenericOperationStatuses } from "../models/GenericOperationStatuses";
import { Response } from "../models/response";
import { ResponseWithData } from "../models/responseWithData";
import { 
    SubjectDto, SubjectCreateDto, 
    SessionDto, SessionCreateDto, 
    TermDto, TermCreateDto, 
    ClassLevelDto, ClassLevelCreateDto 
} from "../models/academic";
import api from "../api/axios";

const API_BASE_URL = "v1/academic-structure";

export const academicStructureService = {
    // Subjects
    async getSubjects(): Promise<ResponseWithData<SubjectDto[], GenericOperationStatuses>> {
        const response = await api.get(`${API_BASE_URL}/subjects`);
        return response.data;
    },

    async createSubject(data: SubjectCreateDto): Promise<ResponseWithData<SubjectDto, GenericOperationStatuses>> {
        const response = await api.post(`${API_BASE_URL}/subjects`, data);
        return response.data;
    },

    // Sessions
    async getSessions(): Promise<ResponseWithData<SessionDto[], GenericOperationStatuses>> {
        const response = await api.get(`${API_BASE_URL}/sessions`);
        return response.data;
    },

    async createSession(data: SessionCreateDto): Promise<ResponseWithData<SessionDto, GenericOperationStatuses>> {
        const response = await api.post(`${API_BASE_URL}/sessions`, data);
        return response.data;
    },

    async setActiveSession(sessionId: string): Promise<Response<GenericOperationStatuses>> {
        const response = await api.patch(`${API_BASE_URL}/sessions/${sessionId}/active`);
        return response.data;
    },

    // Terms
    async getTermsBySession(sessionId: string): Promise<ResponseWithData<TermDto[], GenericOperationStatuses>> {
        const response = await api.get(`${API_BASE_URL}/sessions/${sessionId}/terms`);
        return response.data;
    },

    async createTerm(data: TermCreateDto): Promise<ResponseWithData<TermDto, GenericOperationStatuses>> {
        const response = await api.post(`${API_BASE_URL}/terms`, data);
        return response.data;
    },

    // Class Levels
    async getClassLevels(): Promise<ResponseWithData<ClassLevelDto[], GenericOperationStatuses>> {
        const response = await api.get(`${API_BASE_URL}/classes`);
        return response.data;
    },

    async createClassLevel(data: ClassLevelCreateDto): Promise<ResponseWithData<ClassLevelDto, GenericOperationStatuses>> {
        const response = await api.post(`${API_BASE_URL}/classes`, data);
        return response.data;
    },

    async updateClassLevel(id: string, data: ClassLevelCreateDto): Promise<ResponseWithData<ClassLevelDto, GenericOperationStatuses>> {
        const response = await api.put(`${API_BASE_URL}/classes/${id}`, data);
        return response.data;
    },

    async deleteClassLevel(id: string): Promise<Response<GenericOperationStatuses>> {
        const response = await api.delete(`${API_BASE_URL}/classes/${id}`);
        return response.data;
    }
};
