namespace PublicQ.Application.Models.Reporting;

/// <summary>
/// Student progress report details
/// </summary>
public class StudentReportDto
{
    /// <summary>
    /// The unique identifier of the student
    /// </summary>
    public string StudentId { get; set; } = string.Empty;

    /// <summary>
    /// Student's display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Optional: Student admission number.
    /// </summary>
    public string? AdmissionNumber { get; set; }

    /// <summary>
    /// Total number of assignments assigned to this student
    /// </summary>
    public int TotalAssignments { get; set; }

    /// <summary>
    /// Number of assignments completed by this student
    /// </summary>
    public int CompletedAssignments { get; set; }

    /// <summary>
    /// Number of assignments in progress
    /// </summary>
    public int InProgressAssignments { get; set; }

    /// <summary>
    /// Number of assignments not yet started
    /// </summary>
    public int NotStartedAssignments { get; set; }

    /// <summary>
    /// Overall average score across all assignments
    /// </summary>
    public decimal? OverallAverageScore { get; set; }

    /// <summary>
    /// Total time spent on all assignments (in minutes)
    /// </summary>
    public int TotalTimeSpentMinutes { get; set; }
    
    /// <summary>
    /// Detailed progress for each assignment
    /// </summary>
    public List<StudentAssignmentReportDto> AssignmentProgress { get; set; } = [];

    /// <summary>
    /// Total number of times the student switched tabs or minimized the browser across all assignments.
    /// </summary>
    public int TotalTabSwitchCount { get; set; }

    /// <summary>
    /// The last time the student switched tabs or lost focus on the browser across any assignment.
    /// </summary>
    public DateTime? LastTabSwitchAtUtc { get; set; }
}
