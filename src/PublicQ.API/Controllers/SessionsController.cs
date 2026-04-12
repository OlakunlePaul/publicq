using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PublicQ.API.Helpers;
using PublicQ.API.Models.Validators;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Application.Models.Session;
using PublicQ.Shared;

namespace PublicQ.API.Controllers;

/// <summary>
/// SessionController handles endpoints related to user sessions and their interactions within groups.
/// </summary>
/// <param name="sessionService"></param>
[ApiController] 
[Route($"{Constants.ControllerRoutePrefix}/[controller]")]
public class SessionsController(ISessionService sessionService) : ControllerBase
{
    /// <summary>
    /// Fetches the list of group members along with their current module statuses for a specific student and group.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="studentAssignmentId">Student Assignment ID</param>
    /// <param name="groupId">Group ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns list of <see cref="GroupMemberStateWithUserProgressDto"/> wrapped in <see cref="Response{TData, TStatus}"/></returns>
    [HttpGet("{studentId}/assignment/{studentAssignmentId:guid}/group/{groupId:guid}/members")]
    public async Task<IActionResult> GetGroupMembersAsync(
        [FromRoute] string studentId,
        [FromRoute] Guid studentAssignmentId,
        [FromRoute] Guid groupId, 
        CancellationToken cancellationToken)
    {
        if (groupId == Guid.Empty)
        {
            return Response<IList<GroupMemberStateWithUserProgressDto>, GenericOperationStatuses>
                .Failure(GenericOperationStatuses.BadRequest,
                    "Validation failed.")
                .ToActionResult();
        }
        
        var response = await sessionService.GetGroupMemberStatesAsync(
            studentId, 
            studentAssignmentId, 
            groupId, 
            cancellationToken);
        
        return response.ToActionResult();
    }
    
    /// <summary>
    /// Gets the overall state of a group for a specific student and assignment.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="assignmentId">Assignment ID</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpGet("{studentId}/assignment/{assignmentId:guid}/group/state")]
    public async Task<IActionResult> GetGroupStateAsync(
        [FromRoute] string studentId,
        [FromRoute] Guid assignmentId,
        CancellationToken cancellationToken)
    {
        if (assignmentId == Guid.Empty || 
            string.IsNullOrWhiteSpace(studentId))
        {
            return Response<GroupStateDto, GenericOperationStatuses>
                .Failure(GenericOperationStatuses.BadRequest,
                    "Validation failed. Please ensure all IDs are provided and valid.")
                .ToActionResult();
        }
        
        var response = await sessionService.GetGroupStateAsync(
            studentId, 
            assignmentId, 
            cancellationToken);

        if (response.IsSuccess)
        {
            response.Data.ToAbsoluteUrls($"{Request.Scheme}://{Request.Host}");
        }
        
        return response.ToActionResult();
    }
    
    /// <summary>
    /// This endpoint retrieves student-safe module version
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="assignmentId">Assignment ID</param>
    /// <param name="moduleVersionId">Module Version ID</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns <see cref="StudentModuleVersionDto"/> wrapped into <see cref="Response{TData, TStatus}"/></returns>
    [HttpGet("{studentId}/assignment/{assignmentId:guid}/module/version/{moduleVersionId:guid}")]
    public async Task<IActionResult> GetModuleVersionAsync(
        [FromRoute] string studentId,
        [FromRoute] Guid assignmentId,
        [FromRoute] Guid moduleVersionId,
        CancellationToken cancellationToken)
    {
        if (moduleVersionId == Guid.Empty || 
            assignmentId == Guid.Empty || 
            string.IsNullOrWhiteSpace(studentId))
        {
            return Response<IList<GroupMemberStateWithUserProgressDto>, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                    "Validation failed. Please ensure all IDs are provided and valid.")
                .ToActionResult();
        }
        
        var response = await sessionService.GetModuleVersionAsync(
            studentId, 
            assignmentId, 
            moduleVersionId, 
            cancellationToken);
        
        return response.ToActionResult();
    }

    /// <summary>
    /// Gets module progress for a student in a specific assignment and module.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="assignmentId">Assignment ID</param>
    /// <param name="moduleId">Module Id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns <see cref="ModuleProgressDto"/> wrapped in <see cref="Response{TData, TStatus}"/></returns>
    [HttpGet("{studentId}/assignment/{assignmentId:guid}/module/{moduleId:guid}/progress")]
    public async Task<IActionResult> GetModuleProgressAsync(
        string studentId,
        Guid assignmentId,
        Guid moduleId,
        CancellationToken cancellationToken)
    {
        if (assignmentId == Guid.Empty || 
            moduleId == Guid.Empty ||
            string.IsNullOrWhiteSpace(studentId))
        {
            return Response<ModuleProgressDto, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                    "Validation failed. Please ensure all IDs are provided and valid.")
                .ToActionResult();
        }
        
        var response = await sessionService.GetModuleProgressAsync(
            studentId, 
            assignmentId,
            moduleId,
            cancellationToken);
        
        return response.ToActionResult();
    }
    
    /// <summary>
    /// Creates module progress for a student in a specific assignment and module.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="assignmentId">Assignment ID</param>
    /// <param name="moduleId">Module ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns <see cref="ModuleProgressDto"/> wrapped in <see cref="Response{TData, TStatus}"/></returns>
    [HttpPost("{studentId}/assignment/{assignmentId:guid}/module/{moduleId:guid}/progress")]
    public async Task<IActionResult> CreateModuleProgressAsync(
        string studentId,
        Guid assignmentId,
        Guid moduleId,
        CancellationToken cancellationToken)
    {
        if (moduleId == Guid.Empty || 
            assignmentId == Guid.Empty || 
            string.IsNullOrWhiteSpace(studentId))
        {
            return Response<ModuleProgressDto, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                    "Validation failed. Please ensure all IDs are provided and valid.")
                .ToActionResult();
        }
        
        var response = await sessionService.CreateModuleProgressAsync(
            studentId, 
            assignmentId, 
            moduleId, 
            cancellationToken);
        
        return response.ToActionResult();
    }

    /// <summary>
    /// Submits an answer for a specific question within a user's progress session.
    /// </summary>
    /// <param name="userProgressId">User progress ID</param>
    /// <param name="dto"><see cref="QuestionResponseOperationDto"/></param>
    /// <param name="validator">Request validator <see cref="QuestionResponseOperationDtoValidator"/></param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns the <see cref="GenericOperationStatuses"/> wrapped into <see cref="Response{TStatus}"/></returns>
    [HttpPost("progress/{userProgressId:guid}/answer")]
    public async Task<IActionResult> SubmitAnswerAsync(
        [FromRoute] Guid userProgressId,
        [FromBody] QuestionResponseOperationDto dto,
        [FromServices] IValidator<QuestionResponseOperationDto> validator,
        CancellationToken cancellationToken)
    {
        if (userProgressId == Guid.Empty)
        {
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                    "Validation failed. Please ensure all IDs are provided and valid.")
                .ToActionResult();
        }
        
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                "Validation failed.",
                validationResult.Errors.Select(e => e.ErrorMessage).ToList())
                .ToActionResult();
        }
        
        var response = await sessionService.SubmitAnswerAsync(
            userProgressId, 
            dto, 
            cancellationToken);
        
        return response.ToActionResult();
    }
    
    /// <summary>
    /// Completes the module for a user's progress session.
    /// </summary>
    /// <param name="userProgressId">User's progress</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns <see cref="GenericOperationStatuses"/> wrapped into <see cref="Response{TStatus}"/></returns>
    [HttpPost("progress/{userProgressId:guid}/complete")]
    public async Task<IActionResult> CompleteModuleAsync(
        [FromRoute] Guid userProgressId,
        CancellationToken cancellationToken)
    {
        if (userProgressId == Guid.Empty)
        {
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                    "Validation failed. Please ensure all IDs are provided and valid.")
                .ToActionResult();
        }
        
        var response = await sessionService.CompleteModuleAsync(
            userProgressId, 
            cancellationToken);
        
        return response.ToActionResult();
    }

    /// <summary>
    /// Updates the IsCorrect flag on a question response (for manual essay marking).
    /// </summary>
    /// <param name="responseId">Question response ID</param>
    /// <param name="isCorrect">Whether the response is marked as correct</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns <see cref="GenericOperationStatuses"/> wrapped into <see cref="Response{TStatus}"/></returns>
    [HttpPatch("responses/{responseId:guid}/mark")]
    public async Task<IActionResult> UpdateQuestionResponseMarkAsync(
        [FromRoute] Guid responseId,
        [FromQuery] bool isCorrect,
        CancellationToken cancellationToken)
    {
        if (responseId == Guid.Empty)
        {
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                    "Validation failed. Please ensure all IDs are provided and valid.")
                .ToActionResult();
        }
        
        var response = await sessionService.UpdateQuestionResponseMarkAsync(
            responseId,
            isCorrect,
            cancellationToken);
        
        return response.ToActionResult();
    }

    /// <summary>
    /// Grants additional time to a student for a specific module progress.
    /// Restricted to Managers and Administrators.
    /// </summary>
    /// <param name="userProgressId">Module progress ID</param>
    /// <param name="minutes">Minutes to add</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns <see cref="GenericOperationStatuses"/> wrapped into <see cref="Response{TStatus}"/></returns>
    [HttpPost("progress/{userProgressId:guid}/extend-time")]
    // [Authorize(Roles = "Administrator,Manager")] // Assuming standard role-based auth is configured
    public async Task<IActionResult> ExtendTimeAsync(
        [FromRoute] Guid userProgressId,
        [FromQuery] int minutes,
        CancellationToken cancellationToken)
    {
        if (userProgressId == Guid.Empty || minutes <= 0)
        {
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                "Validation failed. Please provide a valid progress ID and positive minutes.")
                .ToActionResult();
        }

        var response = await sessionService.ExtendTimeAsync(
            userProgressId,
            minutes,
            cancellationToken);

        return response.ToActionResult();
    }

    /// <summary>
    /// Gets the user progress ID for a student within a specific assignment context.
    /// This is used by teachers to identify active exam sessions for time extension.
    /// </summary>
    [HttpGet("student/{studentId}/assignment-context")]
    public async Task<IActionResult> GetAssignmentContextAsync(
        [FromRoute] string studentId,
        [FromQuery] Guid sessionId,
        [FromQuery] Guid termId,
        [FromQuery] Guid classId,
        [FromQuery] Guid subjectId,
        CancellationToken cancellationToken)
    {
        var response = await sessionService.GetModuleProgressIdAsync(
            studentId, sessionId, termId, classId, subjectId, cancellationToken);

        if (response.IsSuccess)
        {
            return Ok(new { userProgressId = response.Data });
        }

        return response.ToActionResult();
    }

    /// <summary>
    /// Grants additional time to all students currently taking an exam in a specific class and subject.
    /// </summary>
    [HttpPost("bulk-extend-time")]
    public async Task<IActionResult> BulkExtendTimeAsync(
        [FromBody] BulkExtendTimeRequest request,
        CancellationToken cancellationToken)
    {
        var response = await sessionService.BulkExtendTimeAsync(
            request.SessionId, request.TermId, request.ClassId, request.SubjectId, request.Minutes, cancellationToken);

        return response.ToActionResult();
    }
}

public class BulkExtendTimeRequest
{
    public Guid SessionId { get; set; }
    public Guid TermId { get; set; }
    public Guid ClassId { get; set; }
    public Guid SubjectId { get; set; }
    public int Minutes { get; set; }
}

