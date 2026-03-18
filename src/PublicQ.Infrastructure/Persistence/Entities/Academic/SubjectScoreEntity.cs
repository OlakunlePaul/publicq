using System.ComponentModel.DataAnnotations;

namespace PublicQ.Infrastructure.Persistence.Entities.Academic;

/// <summary>
/// Represents the score components for a specific subject within a student's assessment.
/// Implements the 40/60 Test/Exam scoring logic.
/// </summary>
public class SubjectScoreEntity
{
    /// <summary>
    /// Primary key for the score entry.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key linking this score to the broader student assessment.
    /// </summary>
    [Required]
    public Guid StudentAssessmentId { get; set; }

    /// <summary>
    /// Foreign key to the specific subject.
    /// </summary>
    [Required]
    public Guid SubjectId { get; set; }

    /// <summary>
    /// Test score (Continuous Assessment), typically out of 40.
    /// </summary>
    [Range(0, 40)]
    public decimal? TestScore { get; set; }

    /// <summary>
    /// Exam score, typically out of 60.
    /// </summary>
    [Range(0, 60)]
    public decimal? ExamScore { get; set; }

    /// <summary>
    /// Total computed score (TestScore + ExamScore). Max 100.
    /// </summary>
    [Range(0, 100)]
    public decimal? TotalScore { get; set; }

    /// <summary>
    /// Calculated grade based on TotalScore (e.g., A, B, C, F).
    /// </summary>
    [MaxLength(5)]
    public string? Grade { get; set; }
    
    /// <summary>
    /// Calculated remarks for this specific subject (e.g., "Excellent", "Pass").
    /// </summary>
    [MaxLength(100)]
    public string? SubjectRemark { get; set; }

    // Navigation Properties
    public StudentAssessmentEntity StudentAssessment { get; set; } = default!;
    public SubjectEntity Subject { get; set; } = default!;
}
