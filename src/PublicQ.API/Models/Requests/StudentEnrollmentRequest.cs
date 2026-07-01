using System.ComponentModel.DataAnnotations;

namespace PublicQ.API.Models.Requests;

/// <summary>
/// Request payload for adding or updating a student's enrollment in a term.
/// </summary>
public class StudentEnrollmentRequest
{
    /// <summary>
    /// The session ID (e.g. 2025/2026 Academic Session).
    /// </summary>
    [Required]
    public Guid SessionId { get; set; }

    /// <summary>
    /// The term ID (e.g. First Term).
    /// </summary>
    [Required]
    public Guid TermId { get; set; }

    /// <summary>
    /// The class level ID (e.g. JSS1). Optional.
    /// </summary>
    public Guid? ClassLevelId { get; set; }
}
