using Microsoft.AspNetCore.Authorization;

namespace PublicQ.Infrastructure.Authorization;

/// <summary>
/// Requirement for permission-based authorization.
/// </summary>
/// <param name="permission">Permission name</param>
public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
