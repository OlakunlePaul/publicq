namespace PublicQ.Application.Models.Reporting;

/// <summary>
/// Individual student report details
/// </summary>
public class IndividualStudentReportDto
{
    /// <summary>
    /// Gets or sets the unique identifier for this student assignment.
    /// </summary>
    /// <value>A GUID that uniquely identifies this student-assignment relationship.</value>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the student assigned to this assignment.
    /// References the student who must complete the assigned modules.
    /// </summary>
    /// <value>A string representing the unique identifier of the student.</value>
    /// <remarks>
    /// This is typically the user ID from the identity system (e.g., ASP.NET Core Identity).
    /// The same student can have multiple StudentAssignmentEntity records for different assignments.
    /// </remarks>
    public string StudentId { get; set; }
    
    /// <summary>
    /// Student's display name at the time of assignment.
    /// This redundantly stores the name to preserve historical accuracy,
    /// even if the user's name changes later or in case of user deletion in the identity system.
    /// </summary>
    public string StudentDisplayName { get; set; }

    /// <summary>
    /// Optional: Student admission number.
    /// </summary>
    public string? AdmissionNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the student is locked out of the assignment.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Number of times the student switched tabs or minimized the browser during the exam.
    /// </summary>
    public int TabSwitchCount { get; set; }

    /// <summary>
    /// The timestamp of the last recorded browser focus loss.
    /// </summary>
    public DateTime? LastTabSwitchAtUtc { get; set; }
}
