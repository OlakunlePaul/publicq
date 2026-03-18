using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PublicQ.Infrastructure.Persistence.Entities;

/// <summary>
/// Links an Identity role to a specific permission.
/// </summary>
public class RolePermissionLink
{
    [Required]
    public string RoleId { get; set; } = string.Empty;

    [Required]
    public Guid PermissionId { get; set; }

    public virtual IdentityRole? Role { get; set; }
    public virtual Permission? Permission { get; set; }
}
