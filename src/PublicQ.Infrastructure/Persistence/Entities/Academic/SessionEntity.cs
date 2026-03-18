using System.ComponentModel.DataAnnotations;

namespace PublicQ.Infrastructure.Persistence.Entities.Academic;

/// <summary>
/// Represents an academic session (e.g., 2023/2024).
/// This provides the top-level temporal context for school operations.
/// </summary>
public class SessionEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the session.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the session (e.g., "2023/2024", "Fall 2024").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of the academic session.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the expected end date of the academic session.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Indicates if this is the currently active session for the school.
    /// Usually, only one session is active at a time to determine current operations.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Navigation property: the terms contained within this session.
    /// </summary>
    public ICollection<TermEntity> Terms { get; set; } = new List<TermEntity>();
}
