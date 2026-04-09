namespace PublicQ.Application.Models.Assignment;

/// <summary>
/// Student assignment data transfer object.
/// </summary>
public class StudentAssignmentDto : AssignmentBaseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the student assignment.
    /// </summary>
    public Guid Id { get; set; }
 
    /// <summary>
    /// Student assignment that associates Assignments and Students.
    /// </summary>
    public Guid StudentAssignmentId { get; set; }
    
    /// <summary>
    /// Indicates whether the assignment is published and visible to students.
    /// </summary>
    public bool IsPublished { get; set; }
    
    /// <summary>
    /// Group identifier containing the assessment modules.
    /// </summary>
    public Guid GroupId { get; set; }
  
    /// <summary>
    /// Gets or sets the name of the group containing the assessment modules.
    /// </summary>
    public string GroupTitle { get; set; }

    /// <summary>
    /// Indicates whether the student has fully completed this assignment.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Indicates whether this assignment is automatically locked because an earlier active assignment is not yet completed.
    /// </summary>
    public bool IsProgressionLocked { get; set; }
}
