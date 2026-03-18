using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PublicQ.Application.Interfaces;
using PublicQ.Infrastructure.Persistence;
using PublicQ.Infrastructure.Persistence.Entities;

namespace PublicQ.Infrastructure.Services;

/// <summary>
/// Implementation of the permission management service.
/// </summary>
public class PermissionService(
    ApplicationDbContext dbContext,
    RoleManager<IdentityRole> roleManager) : IPermissionService
{
    private static readonly List<(string Name, string Description)> DefaultPermissions =
    [
        ("Users.View", "View members and staff"),
        ("Users.Manage", "Create and edit users"),
        ("Security.Settings", "Manage system security and passwords"),
        ("Results.View", "View academic results"),
        ("Results.Edit", "Enter and edit student scores"),
        ("Results.Approve", "Moderators/Admins only: Approve and publish results"),
        ("Academic.Settings", "Manage subjects, classes, and sessions"),
        ("Admissions.Settings", "Manage admission number format"),
        ("Settings.Branding", "Manage school profile and logo"),
        ("Communications.Send", "Send emails and SMS notifications"),
        ("System.Settings", "Critical system and technical configuration (Super Admin only)")
    ];

    public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
    {
        return await dbContext.Permissions
            .Select(p => new PermissionDto(p.Id, p.Name, p.Description))
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null) return [];

        return await dbContext.RolePermissionLinks
            .Where(l => l.RoleId == role.Id)
            .Select(l => l.Permission!.Name)
            .ToListAsync();
    }

    public async Task UpdateRolePermissionsAsync(string roleName, IEnumerable<string> permissionNames)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null) throw new ArgumentException($"Role {roleName} not found.");

        // Remove existing links
        var existingLinks = await dbContext.RolePermissionLinks
            .Where(l => l.RoleId == role.Id)
            .ToListAsync();
        
        dbContext.RolePermissionLinks.RemoveRange(existingLinks);

        // Add new links
        var permissions = await dbContext.Permissions
            .Where(p => permissionNames.Contains(p.Name))
            .ToListAsync();

        foreach (var permission in permissions)
        {
            dbContext.RolePermissionLinks.Add(new RolePermissionLink
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SeedPermissionsAsync()
    {
        // 1. Ensure all default permissions exist
        var existingPermissions = await dbContext.Permissions.ToListAsync();
        
        foreach (var (name, description) in DefaultPermissions)
        {
            if (!existingPermissions.Any(p => p.Name == name))
            {
                dbContext.Permissions.Add(new Permission
                {
                    Name = name,
                    Description = description
                });
            }
        }
        
        await dbContext.SaveChangesAsync();

        // 2. Link Administrator role to all permissions if not already linked
        var adminRole = await roleManager.FindByNameAsync("Administrator");
        if (adminRole != null)
        {
            var adminPermissions = await dbContext.RolePermissionLinks
                .Where(l => l.RoleId == adminRole.Id)
                .Select(l => l.PermissionId)
                .ToListAsync();

            var allPermissionIds = await dbContext.Permissions.Select(p => p.Id).ToListAsync();
            
            foreach (var permissionId in allPermissionIds)
            {
                if (!adminPermissions.Contains(permissionId))
                {
                    dbContext.RolePermissionLinks.Add(new RolePermissionLink
                    {
                        RoleId = adminRole.Id,
                        PermissionId = permissionId
                    });
                }
            }
            
            await dbContext.SaveChangesAsync();
        }
        
        await dbContext.SaveChangesAsync();

        // 3. Seed default permissions for other roles
        
        // --- Manager: Full school operations except Security/System ---
        var managerRole = await roleManager.FindByNameAsync("Manager");
        if (managerRole != null)
        {
            var managerPermissionNames = new[] 
            { 
                "Users.View", "Users.Manage", "Results.View", "Results.Edit", 
                "Results.Approve", "Academic.Settings", "Admissions.Settings", 
                "Settings.Branding", "Communications.Send" 
            };
            await AssignPermissionsToRole(managerRole.Id, managerPermissionNames);
        }

        // --- Teacher: Academic focused ---
        var teacherRole = await roleManager.FindByNameAsync("Teacher");
        if (teacherRole != null)
        {
            var teacherPermissionNames = new[] { "Users.View", "Results.View", "Results.Edit" };
            await AssignPermissionsToRole(teacherRole.Id, teacherPermissionNames);
        }

        // --- Parent: View only ---
        var parentRole = await roleManager.FindByNameAsync("Parent");
        if (parentRole != null)
        {
            var parentPermissionNames = new[] { "Results.View" };
            await AssignPermissionsToRole(parentRole.Id, parentPermissionNames);
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task AssignPermissionsToRole(string roleId, string[] permissionNames)
    {
        var existingLinks = await dbContext.RolePermissionLinks
            .Where(l => l.RoleId == roleId)
            .Include(l => l.Permission)
            .ToListAsync();

        var permissionsToAdd = await dbContext.Permissions
            .Where(p => permissionNames.Contains(p.Name))
            .ToListAsync();

        foreach (var permission in permissionsToAdd)
        {
            if (!existingLinks.Any(l => l.PermissionId == permission.Id))
            {
                dbContext.RolePermissionLinks.Add(new RolePermissionLink
                {
                    RoleId = roleId,
                    PermissionId = permission.Id
                });
            }
        }
    }
}
