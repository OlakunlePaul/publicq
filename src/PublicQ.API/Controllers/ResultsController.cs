using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicQ.API.Helpers;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Application.Models.Academic;
using PublicQ.Domain.Enums;
using PublicQ.Shared;

namespace PublicQ.API.Controllers;

/// <summary>
/// API controller for managing academic results, bulk score entry, and moderation.
/// Accessible by Teachers, Contributors, Managers, and Administrators.
/// </summary>
[ApiController]
[Authorize]
[Route($"{Constants.ControllerRoutePrefix}/[controller]")]
public class ResultsController(IResultService resultService) : ControllerBase
{
    /// <summary>
    /// Uploads and parses a result CSV file.
    /// </summary>
    [HttpPost("upload")]
    [Authorize(Constants.ContributorsPolicy)]
    public async Task<IActionResult> UploadCsv(
        [FromForm] IFormFile file,
        [FromForm] Guid sessionId,
        [FromForm] Guid termId,
        [FromForm] Guid classLevelId,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var response = await resultService.UploadResultCsvAsync(stream, sessionId, termId, classLevelId, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Retrieves all student assessments for a specific class in a term/session.
    /// Used to populate the bulk entry grid.
    /// </summary>
    [HttpGet("class")]
    [Authorize(Constants.ContributorsPolicy)]
    public async Task<IActionResult> GetClassAssessmentsAsync(
        [FromQuery] Guid sessionId,
        [FromQuery] Guid termId,
        [FromQuery] Guid classLevelId,
        CancellationToken cancellationToken)
    {
        var response = await resultService.GetClassAssessmentsAsync(sessionId, termId, classLevelId, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Retrieves full details of a specific assessment (Report Card traits and scores).
    /// </summary>
    [HttpGet("report-card/{assessmentId:guid}")]
    public async Task<IActionResult> GetAssessmentDetailsAsync(
        Guid assessmentId,
        CancellationToken cancellationToken)
    {
        var response = await resultService.GetAssessmentDetailsAsync(assessmentId, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Gets results for all children of a parent.
    /// </summary>
    [HttpGet("parent/children")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetParentChildrenResults(CancellationToken cancellationToken)
    {
        var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(parentId)) return Unauthorized();

        var response = await resultService.GetParentChildrenResultsAsync(parentId, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Saves a batch of scores entered by a teacher for a specific subject.
    /// </summary>
    [HttpPost("bulk-scores")]
    [Authorize(Constants.ContributorsPolicy)]
    public async Task<IActionResult> SaveBulkScoresAsync(
        [FromBody] BulkScoreEntryDto request,
        CancellationToken cancellationToken)
    {
        var response = await resultService.SaveBulkScoresAsync(request, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Calculates totals, averages, and ranks for a class.
    /// </summary>
    [HttpPost("calculate")]
    [Authorize(Constants.ContributorsPolicy)]
    public async Task<IActionResult> CalculateClassResultsAsync(
        [FromBody] CalculateClassRequest request,
        CancellationToken cancellationToken)
    {
        var response = await resultService.CalculateClassResultsAsync(request.SessionId, request.TermId, request.ClassLevelId, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Updates the moderation status of an individual student assessment.
    /// </summary>
    [HttpPatch("{assessmentId:guid}/status")]
    [Authorize(Constants.ContributorsPolicy)]
    public async Task<IActionResult> UpdateAssessmentStatusAsync(
        Guid assessmentId,
        [FromBody] ModerationStatus newStatus,
        CancellationToken cancellationToken)
    {
        var response = await resultService.UpdateAssessmentStatusAsync(assessmentId, newStatus, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Updates the non-academic details of an assessment.
    /// </summary>
    [HttpPatch("{assessmentId:guid}/details")]
    [Authorize(Constants.ContributorsPolicy)]
    public async Task<IActionResult> UpdateAssessmentDetailsAsync(
        Guid assessmentId,
        [FromBody] UpdateAssessmentDetailsDto request,
        CancellationToken cancellationToken)
    {
        var response = await resultService.UpdateAssessmentDetailsAsync(assessmentId, request, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Toggles the lock status of an assessment.
    /// </summary>
    [HttpPatch("{assessmentId:guid}/lock")]
    [Authorize(Constants.ContributorsPolicy)]
    public async Task<IActionResult> ToggleAssessmentLockAsync(
        Guid assessmentId,
        [FromBody] LockAssessmentRequest request,
        CancellationToken cancellationToken)
    {
        var response = await resultService.ToggleAssessmentLockAsync(assessmentId, request.IsLocked, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Batch updates the moderation status for an entire class.
    /// </summary>
    [HttpPatch("batch-status")]
    [Authorize(Constants.ModeratorsPolicy)]
    public async Task<IActionResult> BatchUpdateClassStatusAsync(
        [FromBody] BatchStatusRequest request,
        CancellationToken cancellationToken)
    {
        var response = await resultService.BatchUpdateClassStatusAsync(
            request.SessionId, request.TermId, request.ClassLevelId, request.CurrentStatus, request.NewStatus, cancellationToken);
            
        return response.ToActionResult();
    }
}

public record CalculateClassRequest(Guid SessionId, Guid TermId, Guid ClassLevelId);
public record LockAssessmentRequest(bool IsLocked);
public record BatchStatusRequest(Guid SessionId, Guid TermId, Guid ClassLevelId, ModerationStatus CurrentStatus, ModerationStatus NewStatus);
