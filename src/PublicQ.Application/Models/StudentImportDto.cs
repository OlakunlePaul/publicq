namespace PublicQ.Application.Models;

/// <summary>
/// Student DTO
/// <remarks>
/// This DTO is used to do a bulk import and assign students to the
/// assignment.
/// </remarks>
/// </summary>
public class StudentImportDto
{
    /// <summary>
    /// Full name
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Student ID
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Optional: Email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Date of birth of the user.
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Optional: Student admission number.
    /// </summary>
    public string? AdmissionNumber { get; set; }
    
    /// <summary>
    /// Optional: Assignment ID
    /// </summary>
    public Guid? AssignmentId { get; set; }

    /// <summary>
    /// Optional: Class Level ID
    /// </summary>
    public Guid? ClassLevelId { get; set; }
}
