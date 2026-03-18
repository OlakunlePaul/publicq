using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicQ.API.Helpers;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models.Academic;
using PublicQ.Shared;

namespace PublicQ.API.Controllers;

/// <summary>
/// API controller for managing the core academic structure (Sessions, Terms, Classes, Subjects).
/// </summary>
[ApiController]
[Authorize(Constants.ContributorsPolicy)] // Teachers, Contributors, Managers, Admins
[Route($"{Constants.ControllerRoutePrefix}/academic-structure")]
public class AcademicStructureController(IAcademicStructureService apiService) : ControllerBase
{
    // Subjects
    [HttpGet("subjects")]
    public async Task<IActionResult> GetSubjectsAsync(CancellationToken cancellationToken)
    {
        var response = await apiService.GetSubjectsAsync(cancellationToken);
        return response.ToActionResult();
    }

    [Authorize(Constants.ManagersPolicy)] // Only managers/admins should create subjects generally, but depends on school
    [HttpPost("subjects")]
    public async Task<IActionResult> CreateSubjectAsync([FromBody] SubjectCreateDto request, CancellationToken cancellationToken)
    {
        var response = await apiService.CreateSubjectAsync(request, cancellationToken);
        return response.ToActionResult();
    }

    // Sessions
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessionsAsync(CancellationToken cancellationToken)
    {
        var response = await apiService.GetSessionsAsync(cancellationToken);
        return response.ToActionResult();
    }

    [Authorize(Constants.ManagersPolicy)]
    [HttpPost("sessions")]
    public async Task<IActionResult> CreateSessionAsync([FromBody] SessionCreateDto request, CancellationToken cancellationToken)
    {
        var response = await apiService.CreateSessionAsync(request, cancellationToken);
        return response.ToActionResult();
    }

    [Authorize(Constants.ManagersPolicy)]
    [HttpPatch("sessions/{sessionId:guid}/active")]
    public async Task<IActionResult> SetActiveSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var response = await apiService.SetActiveSessionAsync(sessionId, cancellationToken);
        return response.ToActionResult();
    }

    // Terms
    [HttpGet("sessions/{sessionId:guid}/terms")]
    public async Task<IActionResult> GetTermsBySessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var response = await apiService.GetTermsBySessionAsync(sessionId, cancellationToken);
        return response.ToActionResult();
    }

    [Authorize(Constants.ManagersPolicy)]
    [HttpPost("terms")]
    public async Task<IActionResult> CreateTermAsync([FromBody] TermCreateDto request, CancellationToken cancellationToken)
    {
        var response = await apiService.CreateTermAsync(request, cancellationToken);
        return response.ToActionResult();
    }

    // Class Levels
    [HttpGet("classes")]
    public async Task<IActionResult> GetClassLevelsAsync(CancellationToken cancellationToken)
    {
        var response = await apiService.GetClassLevelsAsync(cancellationToken);
        return response.ToActionResult();
    }

    [Authorize(Constants.ManagersPolicy)]
    [HttpPost("classes")]
    public async Task<IActionResult> CreateClassLevelAsync([FromBody] ClassLevelCreateDto request, CancellationToken cancellationToken)
    {
        var response = await apiService.CreateClassLevelAsync(request, cancellationToken);
        return response.ToActionResult();
    }
}
