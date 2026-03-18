using System.ComponentModel.DataAnnotations;

namespace PublicQ.Infrastructure.Persistence.Entities;

/// <summary>
/// Represents a granular permission in the system.
/// </summary>
public class Permission
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250)]
    public string Description { get; set; } = string.Empty;
}
