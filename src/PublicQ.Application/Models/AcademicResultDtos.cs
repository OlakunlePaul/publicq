using PublicQ.Domain.Enums;

namespace PublicQ.Application.Models;

/// <summary>
/// A summary view of a student assessment record.
/// Used for lists, moderation grids, and basic API responses.
/// </summary>
public record AssessmentReportDto
{
    public Guid Id { get; init; }
    public string ExamTakerId { get; init; } = string.Empty;
    public string StudentName { get; init; } = string.Empty;
    public string? AdmissionNumber { get; init; }
    
    public ModerationStatus Status { get; init; }
    public bool IsLockedForParents { get; init; }
    
    public decimal? TotalMarksObtained { get; init; }
    public decimal? AverageScore { get; init; }
    public int? PositionInClass { get; init; }
    public string? OverallGrade { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? PublishedAt { get; init; }
}

/// <summary>
/// Payload for a teacher submitting a batch of scores for a single subject across multiple students.
/// </summary>
public record BulkScoreEntryDto
{
    public Guid SessionId { get; init; }
    public Guid TermId { get; init; }
    public Guid ClassLevelId { get; init; }
    public Guid? SubjectId { get; init; } // Optional if present in individual scores
    
    public List<StudentSubjectScoreDto> Scores { get; init; } = new();
}

/// <summary>
/// Individual student score entry within a bulk submission.
/// </summary>
public record StudentSubjectScoreDto
{
    public string ExamTakerId { get; init; } = string.Empty;
    public Guid? SubjectId { get; init; } // Optional if present in root
    public decimal? TestScore { get; init; }
    public decimal? ExamScore { get; init; }
    public string? SubjectRemark { get; init; }
}
 bitumen

/// <summary>
/// Payload to update the non-academic details of a student's assessment (Affective, Psychomotor, Attendance, Remarks).
/// </summary>
public record UpdateAssessmentDetailsDto
{
    // Attendance
    public int? TimesSchoolOpened { get; init; }
    public int? TimesPresent { get; init; }
    public int? TimesAbsent { get; init; }

    // Affective
    public string? Regularity { get; init; }
    public string? Punctuality { get; init; }
    public string? Neatness { get; init; }
    public string? AttitudeInSchool { get; init; }
    public string? SocialActivities { get; init; }

    // Psychomotor
    public string? IndoorGames { get; init; }
    public string? FieldGames { get; init; }
    public string? TrackGames { get; init; }
    public string? Jumps { get; init; }
    public string? Swims { get; init; }

    // Remarks
    public string? ClassTeacherComment { get; init; }
    public string? HeadTeacherComment { get; init; }
}

/// <summary>
/// Full details of a student's assessment including traits and subject scores.
/// </summary>
public record AssessmentDetailsDto
{
    public Guid Id { get; init; }
    public string ExamTakerId { get; init; } = string.Empty;
    public string StudentName { get; init; } = string.Empty;
    public string? AdmissionNumber { get; init; }
    public ModerationStatus Status { get; init; }
    
    // Academic Summary
    public decimal? TotalMarksObtained { get; init; }
    public decimal? TotalMarksObtainable { get; init; }
    public decimal? AverageScore { get; init; }
    public int? PositionInClass { get; init; }
    public int? NumberInClass { get; init; }
    public string? OverallGrade { get; init; }

    // Attendance
    public int? TimesSchoolOpened { get; init; }
    public int? TimesPresent { get; init; }
    public int? TimesAbsent { get; init; }

    // Affective
    public string? Regularity { get; init; }
    public string? Punctuality { get; init; }
    public string? Neatness { get; init; }
    public string? AttitudeInSchool { get; init; }
    public string? SocialActivities { get; init; }

    // Psychomotor
    public string? IndoorGames { get; init; }
    public string? FieldGames { get; init; }
    public string? TrackGames { get; init; }
    public string? Jumps { get; init; }
    public string? Swims { get; init; }

    // Remarks
    public string? ClassTeacherComment { get; init; }
    public string? HeadTeacherComment { get; init; }

    public List<StudentSubjectScoreDto> SubjectScores { get; init; } = new();
}
