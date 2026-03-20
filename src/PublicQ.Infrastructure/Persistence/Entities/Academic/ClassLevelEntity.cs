using System.ComponentModel.DataAnnotations;

namespace PublicQ.Infrastructure.Persistence.Entities.Academic;

/// <summary>
/// Represents a class level or grade level within a school (e.g., "JSS 1", "Primary 5").
/// Optionally includes subdivisions like arms or sections (e.g., "JSS 1 Green").
/// </summary>
public class ClassLevelEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the class level.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the standardized name of the class (e.g., "JSS 1").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional section or arm identifier (e.g., "Green", "A", "Science").
    /// </summary>
    [MaxLength(50)]
    public string? SectionOrArm { get; set; }

    /// <summary>
    /// A numerical indicator for relative ordering in UI drop-downs (e.g., Pre-K=0, Primary 1=1).
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Optional field to link a class level to a specific grading schema.
    /// If null, the system default or subject-specific defaults might apply.
    /// </summary>
    public Guid? GradingSchemaId { get; set; }

    /// <summary>
    /// Navigation property: the subjects taught in this class level.
    /// </summary>
    public ICollection<SubjectEntity> Subjects { get; set; } = new List<SubjectEntity>();

    /// <summary>
    /// Navigation property to the associated grading schema.
    /// </summary>
    public GradingSchemaEntity? GradingSchema { get; set; }
}
