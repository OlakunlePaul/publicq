using System.ComponentModel.DataAnnotations;

namespace PublicQ.Infrastructure.Persistence.Entities.Academic;

/// <summary>
/// Represents a grading schema that defines how numerical scores are mapped to letter grades and remarks.
/// </summary>
public class GradingSchemaEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Collection of grade ranges defined for this schema.
    /// </summary>
    public ICollection<GradeRangeEntity> GradeRanges { get; set; } = new List<GradeRangeEntity>();
}
