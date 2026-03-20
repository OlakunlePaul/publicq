using System.ComponentModel.DataAnnotations;
using PublicQ.Domain.Enums;

namespace PublicQ.Infrastructure.Persistence.Entities.Academic;

/// <summary>
/// The master record for a student's performance within a specific Term and Session.
/// Contains academic scores, physical traits, behavioral traits, and attendance.
/// This acts as the single source for generating a Report Card.
/// </summary>
public class StudentAssessmentEntity
{
    [Key]
    public Guid Id { get; set; }

    // Core Identifiers
    [Required]
    public string StudentId { get; set; } = default!;
    
    [Required]
    public Guid SessionId { get; set; }
    
    [Required]
    public Guid TermId { get; set; }
    
    [Required]
    public Guid ClassLevelId { get; set; }

    // Overall Status
    /// <summary>
    /// Tracks the review lifecycle: Draft -> Moderated -> Approved -> Published.
    /// </summary>
    public ModerationStatus Status { get; set; } = ModerationStatus.Draft;
    
    /// <summary>
    /// If true, prevents parents from viewing this result (e.g., unpaid fees).
    /// </summary>
    public bool IsLockedForParents { get; set; }

    // Overall Academic Summary
    public decimal? TotalMarksObtained { get; set; }
    public decimal? TotalMarksObtainable { get; set; }
    public decimal? AverageScore { get; set; }
    public int? PositionInClass { get; set; }
    public int? NumberInClass { get; set; }
    
    [MaxLength(5)]
    public string? OverallGrade { get; set; }

    // Attendance & Regularity
    public int? TimesSchoolOpened { get; set; }
    public int? TimesPresent { get; set; }
    public int? TimesAbsent { get; set; }
    
    // Affective Domain / Traits (Typically graded 1-5 or A-E)
    [MaxLength(10)]
    public string? Regularity { get; set; }
    [MaxLength(10)]
    public string? Punctuality { get; set; }
    [MaxLength(10)]
    public string? Neatness { get; set; }
    [MaxLength(10)]
    public string? AttitudeInSchool { get; set; }
    [MaxLength(10)]
    public string? SocialActivities { get; set; }

    // Psychomotor / Sports (Typically graded 1-5 or A-E)
    [MaxLength(10)]
    public string? IndoorGames { get; set; }
    [MaxLength(10)]
    public string? FieldGames { get; set; }
    [MaxLength(10)]
    public string? TrackGames { get; set; }
    [MaxLength(10)]
    public string? Jumps { get; set; }
    [MaxLength(10)]
    public string? Swims { get; set; }

    // Remarks
    [MaxLength(500)]
    public string? ClassTeacherComment { get; set; }
    [MaxLength(500)]
    public string? HeadTeacherComment { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }

    // Navigation Properties
    public SessionEntity Session { get; set; } = default!;
    public TermEntity Term { get; set; } = default!;
    public ClassLevelEntity ClassLevel { get; set; } = default!;
    public StudentEntity Student { get; set; } = default!;
    
    /// <summary>
    /// The individual subject scores that make up this assessment.
    /// </summary>
    public ICollection<SubjectScoreEntity> SubjectScores { get; set; } = new List<SubjectScoreEntity>();
}
