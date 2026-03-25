namespace PublicQ.Application.Models;

/// <summary>
/// Student login request model.
/// </summary>
public class StudentLoginRequest
{
    /// <summary>
    /// Student ID or Admission Number.
    /// </summary>
    public required string AdmissionId { get; set; }
}
