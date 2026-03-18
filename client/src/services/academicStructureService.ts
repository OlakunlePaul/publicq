import { GenericOperationStatuses } from "../models/GenericOperationStatuses";
import { Response } from "../models/response";
import { ResponseWithData } from "../models/responseWithData";
import { 
    SubjectDto, SubjectCreateDto, 
    SessionDto, SessionCreateDto, 
    TermDto, TermCreateDto, 
    ClassLevelDto, ClassLevelCreateDto 
} from "../models/academic";
import { AccessToken } from "../models/accessToken";

const API_BASE_URL = "/api/v1/academic-structure";

// Helper function properly typed
async function fetchWithAuth(url: string, options: RequestInit = {}): Promise<any> {
    const token = localStorage.getItem("token");
    let AuthToken: AccessToken | null = null;
    if (token) {
        AuthToken = JSON.parse(token);
    }
    
    const headers = new Headers(options.headers || {});
    if (AuthToken?.accessToken) {
        headers.set("Authorization", `Bearer ${AuthToken.accessToken}`);
    }
    
    if (!headers.has("Content-Type") && !(options.body instanceof FormData)) {
        headers.set("Content-Type", "application/json");
    }

    const response = await window.fetch(url, { ...options, headers });
    
    // Some 204 No Content responses don't have JSON
    if (response.status === 204) return null;
    
    return await response.json();
}

export const academicStructureService = {
    // Subjects
    async getSubjects(): Promise<ResponseWithData<SubjectDto[], GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/subjects`);
    },

    async createSubject(data: SubjectCreateDto): Promise<ResponseWithData<SubjectDto, GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/subjects`, {
            method: "POST",
            body: JSON.stringify(data),
        });
    },

    // Sessions
    async getSessions(): Promise<ResponseWithData<SessionDto[], GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/sessions`);
    },

    async createSession(data: SessionCreateDto): Promise<ResponseWithData<SessionDto, GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/sessions`, {
            method: "POST",
            body: JSON.stringify(data),
        });
    },

    async setActiveSession(sessionId: string): Promise<Response<GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/sessions/${sessionId}/active`, {
            method: "PATCH",
        });
    },

    // Terms
    async getTermsBySession(sessionId: string): Promise<ResponseWithData<TermDto[], GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/sessions/${sessionId}/terms`);
    },

    async createTerm(data: TermCreateDto): Promise<ResponseWithData<TermDto, GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/terms`, {
            method: "POST",
            body: JSON.stringify(data),
        });
    },

    // Class Levels
    async getClassLevels(): Promise<ResponseWithData<ClassLevelDto[], GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/classes`);
    },

    async createClassLevel(data: ClassLevelCreateDto): Promise<ResponseWithData<ClassLevelDto, GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/classes`, {
            method: "POST",
            body: JSON.stringify(data),
        });
    }
};
