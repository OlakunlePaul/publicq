using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicQ.API.Helpers;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Domain.Enums;
using PublicQ.Shared;

namespace PublicQ.API.Controllers;

/// <summary>
/// API controller for managing academic results, bulk score entry, and moderation.
/// Accessible by Teachers, Contributors, Managers, and Administrators.
/// </summary>
[ApiController]
[Authorize(Constants.ContributorsPolicy)]
[Route($"{Constants.ControllerRoutePrefix}/[controller]")]
public class ResultsController(IResultService resultService) : ControllerBase
{
    /// <summary>
    /// Retrieves all student assessments for a specific class in a term/session.
    /// Used to populate the bulk entry grid.
    /// </summary>
    [HttpGet("class-assessments")]
    public async Task<IActionResult> GetClassAssessmentsAsync(
        [FromQuery] Guid sessionId,
        [FromQuery] Guid termId,
        [FromQuery] Guid classLevelId,
        CancellationToken cancellationToken)
    {
        if (sessionId == Guid.Empty || termId == Guid.Empty || classLevelId == Guid.Empty)
        {
            return BadRequest("Session ID, Term ID, and Class Level ID are required.");
        }

        var response = await resultService.GetClassAssessmentsAsync(sessionId, termId, classLevelId, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Retrieves full details of a specific assessment (Report Card traits and scores).
    /// </summary>
    [HttpGet("{assessmentId:guid}")]
    public async Task<IActionResult> GetAssessmentDetailsAsync(
        Guid assessmentId,
        CancellationToken cancellationToken)
    {
        if (assessmentId == Guid.Empty)
        {
            return BadRequest("Assessment ID is required.");
        }

        var response = await resultService.GetAssessmentDetailsAsync(assessmentId, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Saves a batch of scores entered by a teacher for a specific subject.
    /// </summary>
    [HttpPost("bulk-scores")]
    public async Task<IActionResult> SaveBulkScoresAsync(
        [FromBody] BulkScoreEntryDto request,
        CancellationToken cancellationToken)
    {
        if (request.SessionId == Guid.Empty || request.TermId == Guid.Empty || request.ClassLevelId == Guid.Empty || request.SubjectId == Guid.Empty)
        {
            return BadRequest("Invalid bulk score entry payload.");
        }

        var response = await resultService.SaveBulkScoresAsync(request, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Calculates totals, averages, and ranks for a class.
    /// Triggered after score entry is complete before submitting to moderation.
    /// </summary>
    [HttpPost("calculate-class")]
    public async Task<IActionResult> CalculateClassResultsAsync(
        [FromQuery] Guid sessionId,
        [FromQuery] Guid termId,
        [FromQuery] Guid classLevelId,
        CancellationToken cancellationToken)
    {
        if (sessionId == Guid.Empty || termId == Guid.Empty || classLevelId == Guid.Empty)
        {
            return BadRequest("Session ID, Term ID, and Class Level ID are required.");
        }

        var response = await resultService.CalculateClassResultsAsync(sessionId, termId, classLevelId, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Updates the moderation status of an individual student assessment.
    /// </summary>
    [HttpPatch("{assessmentId:guid}/status")]
    public async Task<IActionResult> UpdateAssessmentStatusAsync(
        Guid assessmentId,
        [FromQuery] ModerationStatus newStatus,
        CancellationToken cancellationToken)
    {
        if (assessmentId == Guid.Empty)
        {
            return BadRequest("Assessment ID is required.");
        }

        var response = await resultService.UpdateAssessmentStatusAsync(assessmentId, newStatus, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Updates the non-academic details of an assessment (Affective, Psychomotor, Attendance, Remarks).
    /// </summary>
    [HttpPatch("{assessmentId:guid}/details")]
    public async Task<IActionResult> UpdateAssessmentDetailsAsync(
        Guid assessmentId,
        [FromBody] UpdateAssessmentDetailsDto request,
        CancellationToken cancellationToken)
    {
        if (assessmentId == Guid.Empty)
        {
            return BadRequest("Assessment ID is required.");
        }

        var response = await resultService.UpdateAssessmentDetailsAsync(assessmentId, request, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Toggles the lock status of an assessment (prevents parents from viewing).
    /// </summary>
    [HttpPatch("{assessmentId:guid}/lock")]
    public async Task<IActionResult> ToggleAssessmentLockAsync(
        Guid assessmentId,
        [FromQuery] bool isLocked,
        CancellationToken cancellationToken)
    {
        if (assessmentId == Guid.Empty)
        {
            return BadRequest("Assessment ID is required.");
        }

        var response = await resultService.ToggleAssessmentLockAsync(assessmentId, isLocked, cancellationToken);
        return response.ToActionResult();
    }

    /// <summary>
    /// Batch updates the moderation status for an entire class.
    /// E.g., Moderated -> Approved -> Published.
    /// </summary>
    [HttpPatch("class-status")]
    public async Task<IActionResult> BatchUpdateClassStatusAsync(
        [FromQuery] Guid sessionId,
        [FromQuery] Guid termId,
        [FromQuery] Guid classLevelId,
        [FromQuery] ModerationStatus currentStatus,
        [FromQuery] ModerationStatus newStatus,
        CancellationToken cancellationToken)
    {
        if (sessionId == Guid.Empty || termId == Guid.Empty || classLevelId == Guid.Empty)
        {
            return BadRequest("Session ID, Term ID, and Class Level ID are required.");
        }

        var response = await resultService.BatchUpdateClassStatusAsync(
            sessionId, termId, classLevelId, currentStatus, newStatus, cancellationToken);
            
        return response.ToActionResult();
    }
}
