using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models.Academic;
using PublicQ.Domain.Enums;
using System.Security.Claims;

namespace PublicQ.API.Controllers;

[ApiController]
[Route("api/v1/results")]
[Authorize]
public class ResultController(IResultService resultService) : ControllerBase
{
    [HttpPost("upload")]
    [Authorize(Policy = "Permission:Results.Edit")]
    public async Task<ActionResult<ResultUploadResponse>> UploadCsv(
        [FromForm] IFormFile file,
        [FromForm] Guid sessionId,
        [FromForm] Guid termId,
        [FromForm] Guid classLevelId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var result = await resultService.UploadResultCsvAsync(stream, sessionId, termId, classLevelId);
        
        if (result.SuccessCount == 0 && result.FailureCount > 0)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("report-card/{assessmentId}")]
    [Authorize(Policy = "Permission:Results.View")]
    public async Task<ActionResult<StudentAssessmentDto>> GetReportCard(Guid assessmentId)
    {
        var result = await resultService.GetStudentAssessmentAsync(assessmentId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("class")]
    [Authorize(Policy = "Permission:Results.View")]
    public async Task<ActionResult<IEnumerable<StudentAssessmentDto>>> GetClassResults(
        [FromQuery] Guid sessionId,
        [FromQuery] Guid termId,
        [FromQuery] Guid classLevelId)
    {
        var results = await resultService.GetClassAssessmentsAsync(sessionId, termId, classLevelId);
        return Ok(results);
    }

    [HttpGet("parent/children")]
    [Authorize(Roles = "Parent")]
    public async Task<ActionResult<IEnumerable<StudentAssessmentDto>>> GetParentChildrenResults()
    {
        var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(parentId)) return Unauthorized();

        var results = await resultService.GetParentChildrenResultsAsync(parentId);
        return Ok(results);
    }

    [HttpPatch("{assessmentId}/status")]
    [Authorize(Policy = "Permission:Results.Approve")]
    public async Task<IActionResult> UpdateStatus(Guid assessmentId, [FromBody] ModerationStatus status)
    {
        await resultService.UpdateStatusAsync(assessmentId, status);
        return NoContent();
    }
}
