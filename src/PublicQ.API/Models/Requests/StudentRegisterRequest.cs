namespace PublicQ.API.Models.Requests;

/// <summary>
/// Student registration request model.
/// </summary>
public class StudentRegisterRequest
{
    /// <summary>
    /// Optional: Student unique identifier.
    /// <remarks>
    /// System will generate a new ID if not provided.
    /// </remarks>
    /// </summary>
    public string? Id { get; set; }
    
    /// <summary>
    /// Optional: Email address of the student.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Date of birth of the user.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Optional: Student admission number.
    /// </summary>
    public string? AdmissionNumber { get; set; }
    
    /// <summary>
    /// Student full name.
    /// </summary>
    public string FullName { get; set; } = null!;

    /// <summary>
    /// Optional: Foreign key reference to the class level.
    /// </summary>
    public Guid? ClassLevelId { get; set; }
}
