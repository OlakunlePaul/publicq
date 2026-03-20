namespace PublicQ.API.Models.Requests;

/// <summary>
/// User registration request model.
/// </summary>
public class UserRegisterRequest
{
    /// <summary>
    /// Full name of the user.
    /// </summary>
    public string FullName { get; set; } = default!;
    
    /// <summary>
    /// Email address of the user.
    /// </summary>
    public string Email { get; set; } = default!;
    
    /// <summary>
    /// Password of the user.
    /// </summary>
    public string Password { get; set; } = default!;
    
    /// <summary>
    /// Date of birth of the user.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Optional: Student admission number.
    /// </summary>
    public string? AdmissionNumber { get; set; }

    /// <summary>
    /// Optional: Foreign key reference to the class level.
    /// </summary>
    public Guid? ClassLevelId { get; set; }
}