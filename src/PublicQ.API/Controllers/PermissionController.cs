using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicQ.Application.Interfaces;
using PublicQ.Shared;

namespace PublicQ.API.Controllers;

/// <summary>
/// Controller for managing system permissions and role assignments.
/// </summary>
[Authorize(Constants.AdminsPolicy)]
[ApiController]
[Route("api/[controller]")]
public class PermissionController(IPermissionService permissionService) : ControllerBase
{
    /// <summary>
    /// Gets all available permissions in the system.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAllPermissions()
    {
        var permissions = await permissionService.GetAllPermissionsAsync();
        return Ok(permissions);
    }

    /// <summary>
    /// Gets permissions assigned to a specific role.
    /// </summary>
    /// <param name="roleName">Role name</param>
    [HttpGet("role/{roleName}")]
    public async Task<ActionResult<IEnumerable<string>>> GetRolePermissions(string roleName)
    {
        var permissions = await permissionService.GetRolePermissionsAsync(roleName);
        return Ok(permissions);
    }

    /// <summary>
    /// Updates permissions for a specific role.
    /// </summary>
    /// <param name="roleName">Role name</param>
    /// <param name="permissionNames">List of permission names to assign</param>
    [HttpPost("role/{roleName}")]
    public async Task<IActionResult> UpdateRolePermissions(string roleName, [FromBody] IEnumerable<string> permissionNames)
    {
        try
        {
            await permissionService.UpdateRolePermissionsAsync(roleName, permissionNames);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
