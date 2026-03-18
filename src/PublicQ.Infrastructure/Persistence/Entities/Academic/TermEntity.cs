using System.ComponentModel.DataAnnotations;

namespace PublicQ.Infrastructure.Persistence.Entities.Academic;

/// <summary>
/// Represents an academic term within a session (e.g., First Term, Fall Semester).
/// </summary>
public class TermEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the term.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the defining academic session.
    /// </summary>
    [Required]
    public Guid SessionId { get; set; }

    /// <summary>
    /// Gets or sets the name of the term (e.g., "FIRST TERM", "Semester 1").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of the term.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the expected end date of the term.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the date the next term begins, used heavily for report cards.
    /// </summary>
    public DateTime? NextTermBegins { get; set; }

    /// <summary>
    /// Gets or sets a description or amount for next term's fees, displayed on report cards.
    /// </summary>
    [MaxLength(200)]
    public string? NextTermBills { get; set; }

    /// <summary>
    /// Indicates if this is the currently active term within the school.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Navigation property to the parent session.
    /// </summary>
    public SessionEntity Session { get; set; } = default!;
}
