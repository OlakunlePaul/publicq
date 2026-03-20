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

const API_BASE_URL = "academic-structure";

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

    async updateSubject(id: string, data: SubjectCreateDto): Promise<ResponseWithData<SubjectDto, GenericOperationStatuses>> {
        const response = await api.put(`${API_BASE_URL}/subjects/${id}`, data);
        return response.data;
    },

    async deleteSubject(id: string): Promise<Response<GenericOperationStatuses>> {
        const response = await api.delete(`${API_BASE_URL}/subjects/${id}`);
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

    async updateSession(id: string, data: SessionCreateDto): Promise<ResponseWithData<SessionDto, GenericOperationStatuses>> {
        const response = await api.put(`${API_BASE_URL}/sessions/${id}`, data);
        return response.data;
    },

    async deleteSession(id: string): Promise<Response<GenericOperationStatuses>> {
        const response = await api.delete(`${API_BASE_URL}/sessions/${id}`);
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

    async updateTerm(id: string, data: TermCreateDto): Promise<ResponseWithData<TermDto, GenericOperationStatuses>> {
        const response = await api.put(`${API_BASE_URL}/terms/${id}`, data);
        return response.data;
    },

    async deleteTerm(id: string): Promise<Response<GenericOperationStatuses>> {
        const response = await api.delete(`${API_BASE_URL}/terms/${id}`);
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
    },

    // Grading Schemas
    async getGradingSchemas(): Promise<ResponseWithData<any[], GenericOperationStatuses>> {
        const response = await api.get(`${API_BASE_URL}/grading-schemas`);
        return response.data;
    },

    async createGradingSchema(data: any): Promise<ResponseWithData<any, GenericOperationStatuses>> {
        const response = await api.post(`${API_BASE_URL}/grading-schemas`, data);
        return response.data;
    },

    async updateGradingSchema(id: string, data: any): Promise<ResponseWithData<any, GenericOperationStatuses>> {
        const response = await api.put(`${API_BASE_URL}/grading-schemas/${id}`, data);
        return response.data;
    },

    async deleteGradingSchema(id: string): Promise<Response<GenericOperationStatuses>> {
        const response = await api.delete(`${API_BASE_URL}/grading-schemas/${id}`);
        return response.data;
    }
};
