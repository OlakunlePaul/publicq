export interface SubjectDto {
    id: string;
    name: string;
    code?: string;
    displayOrder: number;
}

export interface SubjectCreateDto {
    name: string;
    code?: string;
    displayOrder: number;
}

export interface SessionDto {
    id: string;
    name: string;
    startDate?: string;
    endDate?: string;
    isActive: boolean;
}

export interface SessionCreateDto {
    name: string;
    startDate?: string;
    endDate?: string;
    isActive: boolean;
}

export interface TermDto {
    id: string;
    sessionId: string;
    sessionName: string;
    name: string;
    startDate?: string;
    endDate?: string;
    nextTermBegins?: string;
    isActive: boolean;
}

export interface TermCreateDto {
    sessionId: string;
    name: string;
    startDate?: string;
    endDate?: string;
    nextTermBegins?: string;
    isActive: boolean;
}

export interface ClassLevelDto {
    id: string;
    name: string;
    sectionOrArm?: string;
    orderIndex: number;
}

export interface ClassLevelCreateDto {
    name: string;
    sectionOrArm?: string;
    orderIndex: number;
}

// Result DTOs
export interface AssessmentReportDto {
    id: string;
    examTakerId: string;
    studentName: string;
    admissionNumber?: string;
    status: number; // enum ModertionStatus
    isLockedForParents: boolean;
    totalMarksObtained?: number;
    averageScore?: number;
    positionInClass?: number;
    overallGrade?: string;
    createdAt: string;
    publishedAt?: string;
}

export interface BulkScoreEntryDto {
    sessionId: string;
    termId: string;
    classLevelId: string;
    subjectId: string;
    scores: StudentSubjectScoreDto[];
}

export interface StudentSubjectScoreDto {
    examTakerId: string;
    testScore?: number;
    examScore?: number;
    subjectRemark?: string;
}

export enum ModerationStatus {
    Draft = 0,
    Moderated = 1,
    Approved = 2,
    Published = 3
}

export interface UpdateAssessmentDetailsDto {
    timesSchoolOpened?: number;
    timesPresent?: number;
    timesAbsent?: number;

    regularity?: string;
    punctuality?: string;
    neatness?: string;
    attitudeInSchool?: string;
    socialActivities?: string;

    indoorGames?: string;
    fieldGames?: string;
    trackGames?: string;
    jumps?: string;
    swims?: string;

    classTeacherComment?: string;
    headTeacherComment?: string;
}

export interface AssessmentDetailsDto {
    id: string;
    examTakerId: string;
    studentName: string;
    admissionNumber?: string;
    status: number;
    
    totalMarksObtained?: number;
    totalMarksObtainable?: number;
    averageScore?: number;
    positionInClass?: number;
    numberInClass?: number;
    overallGrade?: string;

    timesSchoolOpened?: number;
    timesPresent?: number;
    timesAbsent?: number;

    regularity?: string;
    punctuality?: string;
    neatness?: string;
    attitudeInSchool?: string;
    socialActivities?: string;

    indoorGames?: string;
    fieldGames?: string;
    trackGames?: string;
    jumps?: string;
    swims?: string;

    classTeacherComment?: string;
    headTeacherComment?: string;

    subjectScores: StudentSubjectScoreDto[];
}
