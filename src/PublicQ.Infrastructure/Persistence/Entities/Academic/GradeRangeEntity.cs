using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicQ.Infrastructure.Persistence.Entities.Academic;

/// <summary>
/// Defines a specific grade range (e.g., 70-100 = A) within a grading schema.
/// </summary>
public class GradeRangeEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid GradingSchemaId { get; set; }

    [ForeignKey(nameof(GradingSchemaId))]
    public GradingSchemaEntity? GradingSchema { get; set; }

    /// <summary>
    /// The letter grade or symbol (e.g., "A", "B+", "Distinction").
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// The minimum score for this grade (inclusive).
    /// </summary>
    [Range(0, 100)]
    public int MinScore { get; set; }

    /// <summary>
    /// The maximum score for this grade (inclusive).
    /// </summary>
    [Range(0, 100)]
    public int MaxScore { get; set; }

    /// <summary>
    /// A standardized remark for this grade (e.g., "Excellent", "Fail").
    /// </summary>
    [MaxLength(100)]
    public string Remark { get; set; } = string.Empty;
}
