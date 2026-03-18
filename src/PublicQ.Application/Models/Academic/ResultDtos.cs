using PublicQ.Domain.Enums;

namespace PublicQ.Application.Models.Academic;

public record StudentAssessmentDto(
    Guid Id,
    string ExamTakerId,
    string StudentName,
    string AdmissionNumber,
    string SessionName,
    string TermName,
    string ClassName,
    ModerationStatus Status,
    decimal? TotalMarksObtained,
    decimal? TotalMarksObtainable,
    decimal? AverageScore,
    int? PositionInClass,
    int? NumberInClass,
    string? OverallGrade,
    int? TimesSchoolOpened,
    int? TimesPresent,
    int? TimesAbsent,
    string? ClassTeacherComment,
    string? HeadTeacherComment,
    List<SubjectScoreDto> SubjectScores);

public record SubjectScoreDto(
    Guid Id,
    string SubjectName,
    decimal? TestScore,
    decimal? ExamScore,
    decimal? TotalScore,
    string? Grade,
    string? SubjectRemark);

public record ResultUploadResponse(
    int TotalProcessed,
    int SuccessCount,
    int FailureCount,
    List<string> Errors);
