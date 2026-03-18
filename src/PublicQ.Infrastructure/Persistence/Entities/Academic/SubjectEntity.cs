using System.ComponentModel.DataAnnotations;

namespace PublicQ.Infrastructure.Persistence.Entities.Academic;

/// <summary>
/// Represents a subject taught in the school (e.g., "Mathematics", "English Language").
/// </summary>
public class SubjectEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the subject.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the subject.
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional unique code representing the subject (e.g., "MTH101").
    /// Useful for syncing with external School Management Systems.
    /// </summary>
    [MaxLength(50)]
    public string? Code { get; set; }
    
    /// <summary>
    /// Determines the order in which subjects are displayed on report cards.
    /// </summary>
    public int DisplayOrder { get; set; }
}
