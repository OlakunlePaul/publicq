namespace PublicQ.Application.Interfaces;

/// <summary>
/// Service for managing system permissions and role assignments.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Gets all available permissions in the system.
    /// </summary>
    Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();

    /// <summary>
    /// Gets permissions assigned to a specific role.
    /// </summary>
    /// <param name="roleName">Role name</param>
    Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName);

    /// <summary>
    /// Updates permissions for a specific role.
    /// </summary>
    /// <param name="roleName">Role name</param>
    /// <param name="permissionNames">List of permission names to assign</param>
    Task UpdateRolePermissionsAsync(string roleName, IEnumerable<string> permissionNames);

    /// <summary>
    /// Ensures all default permissions exist in the database.
    /// </summary>
    Task SeedPermissionsAsync();
}

public record PermissionDto(Guid Id, string Name, string Description);
