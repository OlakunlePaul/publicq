using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PublicQ.Infrastructure.Persistence;

namespace PublicQ.Infrastructure.Authorization;

/// <summary>
/// Handler for permission-based authorization.
/// </summary>
public class PermissionHandler(ApplicationDbContext dbContext) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        // For administrators, we bypass permission checks (they have all permissions)
        if (context.User.IsInRole("Administrator"))
        {
            context.Succeed(requirement);
            return;
        }

        // Get the current user's roles
        var roles = context.User.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (roles.Count == 0)
        {
            return;
        }

        // Check if any of the user's roles have the required permission in the database
        var hasPermission = await dbContext.RolePermissionLinks
            .AnyAsync(l => roles.Contains(l.Role!.Name!) && l.Permission!.Name == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
