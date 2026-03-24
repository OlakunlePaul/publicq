export interface SubjectDto {
    id: string;
    name: string;
    code?: string;
    displayOrder: number;
    classLevelIds?: string[];
    classLevelNames?: string[];
}

export interface SubjectCreateDto {
    name: string;
    code?: string;
    displayOrder: number;
    classLevelIds?: string[];
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
    gradingSchemaId?: string;
    subjectIds?: string[];
    subjectNames?: string[];
}

export interface ClassLevelCreateDto {
    name: string;
    sectionOrArm?: string;
    orderIndex: number;
    gradingSchemaId?: string;
    subjectIds?: string[];
}

// Result DTOs
export interface AssessmentReportDto {
    id: string;
    studentId: string;
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
    subjectId?: string;
    scores: StudentSubjectScoreDto[];
}

export interface StudentSubjectScoreDto {
    studentId: string;
    subjectId?: string;
    subjectName?: string;
    testScore?: number;
    examScore?: number;
    totalScore?: number;
    grade?: string;
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
    studentId: string;
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

export interface GradingSchemaDto {
    id: string;
    name: string;
    isActive: boolean;
    gradeRanges: GradeRangeDto[];
}

export interface GradeRangeDto {
    id?: string;
    gradingSchemaId?: string;
    symbol: string;
    minScore: number;
    maxScore: number;
    remark: string;
}

export interface GradingSchemaCreateDto {
    name: string;
    isActive: boolean;
    gradeRanges: GradeRangeDto[];
}

export interface ClassLevelUpdateDto extends ClassLevelCreateDto {
    id: string;
    gradingSchemaId?: string;
}
