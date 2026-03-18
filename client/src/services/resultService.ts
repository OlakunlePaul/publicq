import { GenericOperationStatuses } from "../models/GenericOperationStatuses";
import { Response } from "../models/response";
import { ResponseWithData } from "../models/responseWithData";
import { AssessmentReportDto, BulkScoreEntryDto, ModerationStatus, UpdateAssessmentDetailsDto, AssessmentDetailsDto } from "../models/academic";
import { AccessToken } from "../models/accessToken";

const API_BASE_URL = "/api/v1/results";

// Helper function
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
    if (response.status === 204) return null;
    
    return await response.json();
}

export const resultService = {
    async getClassAssessments(
        sessionId: string,
        termId: string,
        classLevelId: string
    ): Promise<ResponseWithData<AssessmentReportDto[], GenericOperationStatuses>> {
        const queryParams = new URLSearchParams({ sessionId, termId, classLevelId });
        return await fetchWithAuth(`${API_BASE_URL}/class-assessments?${queryParams}`);
    },

    async getAssessmentDetails(assessmentId: string): Promise<ResponseWithData<AssessmentDetailsDto, GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/${assessmentId}`);
    },

    async saveBulkScores(data: BulkScoreEntryDto): Promise<Response<GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/bulk-scores`, {
            method: "POST",
            body: JSON.stringify(data),
        });
    },

    async calculateClassResults(
        sessionId: string,
        termId: string,
        classLevelId: string
    ): Promise<Response<GenericOperationStatuses>> {
        const queryParams = new URLSearchParams({ sessionId, termId, classLevelId });
        return await fetchWithAuth(`${API_BASE_URL}/calculate-class?${queryParams}`, {
            method: "POST",
        });
    },

    async updateAssessmentStatus(
        assessmentId: string,
        newStatus: ModerationStatus
    ): Promise<Response<GenericOperationStatuses>> {
        const queryParams = new URLSearchParams({ newStatus: newStatus.toString() });
        return await fetchWithAuth(`${API_BASE_URL}/${assessmentId}/status?${queryParams}`, {
            method: "PATCH",
        });
    },

    async updateAssessmentDetails(
        assessmentId: string,
        details: UpdateAssessmentDetailsDto
    ): Promise<Response<GenericOperationStatuses>> {
        return await fetchWithAuth(`${API_BASE_URL}/${assessmentId}/details`, {
            method: "PATCH",
            body: JSON.stringify(details),
        });
    },

    async toggleAssessmentLock(
        assessmentId: string,
        isLocked: boolean
    ): Promise<Response<GenericOperationStatuses>> {
        const queryParams = new URLSearchParams({ isLocked: isLocked.toString() });
        return await fetchWithAuth(`${API_BASE_URL}/${assessmentId}/lock?${queryParams}`, {
            method: "PATCH",
        });
    },

    async batchUpdateClassStatus(
        sessionId: string,
        termId: string,
        classLevelId: string,
        currentStatus: ModerationStatus,
        newStatus: ModerationStatus
    ): Promise<Response<GenericOperationStatuses>> {
        const queryParams = new URLSearchParams({ 
            sessionId, 
            termId, 
            classLevelId,
            currentStatus: currentStatus.toString(),
            newStatus: newStatus.toString()
        });
        return await fetchWithAuth(`${API_BASE_URL}/class-status?${queryParams}`, {
            method: "PATCH",
        });
    }
};
