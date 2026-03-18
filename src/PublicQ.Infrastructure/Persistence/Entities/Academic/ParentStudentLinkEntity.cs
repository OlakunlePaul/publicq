using System.ComponentModel.DataAnnotations;

namespace PublicQ.Infrastructure.Persistence.Entities.Academic;

/// <summary>
/// Connects a Parent User (ApplicationUser) to a Student (ExamTakerEntity).
/// This allows a parent to view multiple children's results, 
/// and a child to be linked to multiple parents/guardians.
/// </summary>
public class ParentStudentLinkEntity
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign Key to the Parent user (ApplicationUser ID)
    /// </summary>
    [Required]
    public string ParentId { get; set; } = default!;

    /// <summary>
    /// Foreign Key to the Student user (ExamTakerEntity ID)
    /// </summary>
    [Required]
    public string StudentId { get; set; } = default!;

    /// <summary>
    /// Relationship description (e.g., "Father", "Mother", "Guardian")
    /// </summary>
    [MaxLength(50)]
    public string? Relationship { get; set; }

    // Navigation Properties
    // Using ApplicationUser for Parent (since they need login credentials to access the portal)
    public ApplicationUser Parent { get; set; } = default!;
    
    // Using ExamTakerEntity for the Student
    public ExamTakerEntity Student { get; set; } = default!;
}
