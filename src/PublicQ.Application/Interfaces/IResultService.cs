using PublicQ.Application.Models;
using PublicQ.Application.Models.Academic;
using PublicQ.Domain.Enums;
using PublicQ.Shared;

namespace PublicQ.Application.Interfaces;

/// <summary>
/// Service for managing academic results and report cards.
/// </summary>
public interface IResultService
{
    /// <summary>
    /// Uploads and parses a result CSV file (Mercy's Gate format).
    /// </summary>
    Task<Response<ResultUploadResponse, GenericOperationStatuses>> UploadResultCsvAsync(Stream fileStream, Guid sessionId, Guid termId, Guid classLevelId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a detailed assessment record for report card generation.
    /// </summary>
    Task<Response<StudentAssessmentDto, GenericOperationStatuses>> GetStudentAssessmentAsync(Guid assessmentId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all assessments for a specific term and class.
    /// </summary>
    Task<Response<IEnumerable<StudentAssessmentDto>, GenericOperationStatuses>> GetClassAssessmentsAsync(Guid sessionId, Guid termId, Guid classLevelId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets results for all children of a parent.
    /// </summary>
    Task<Response<IEnumerable<StudentAssessmentDto>, GenericOperationStatuses>> GetParentChildrenResultsAsync(string parentUserId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the moderation status of an assessment.
    /// </summary>
    Task<Response<GenericOperationStatuses>> UpdateStatusAsync(Guid assessmentId, ModerationStatus status, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves full details of a specific assessment (Report Card traits and scores).
    /// </summary>
    Task<Response<AssessmentDetailsDto, GenericOperationStatuses>> GetAssessmentDetailsAsync(Guid assessmentId, CancellationToken cancellationToken);

    /// <summary>
    /// Saves a batch of scores entered by a teacher for a specific subject.
    /// </summary>
    Task<Response<GenericOperationStatuses>> SaveBulkScoresAsync(BulkScoreEntryDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Calculates totals, averages, and ranks for a class.
    /// </summary>
    Task<Response<GenericOperationStatuses>> CalculateClassResultsAsync(Guid sessionId, Guid termId, Guid classLevelId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the moderation status of an individual student assessment.
    /// </summary>
    Task<Response<GenericOperationStatuses>> UpdateAssessmentStatusAsync(Guid assessmentId, ModerationStatus newStatus, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the non-academic details of an assessment (Affective, Psychomotor, Attendance, Remarks).
    /// </summary>
    Task<Response<GenericOperationStatuses>> UpdateAssessmentDetailsAsync(Guid assessmentId, UpdateAssessmentDetailsDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Toggles the lock status of an assessment (prevents parents from viewing).
    /// </summary>
    Task<Response<GenericOperationStatuses>> ToggleAssessmentLockAsync(Guid assessmentId, bool isLocked, CancellationToken cancellationToken);

    /// <summary>
    /// Batch updates the moderation status for an entire class.
    /// </summary>
    Task<Response<GenericOperationStatuses>> BatchUpdateClassStatusAsync(Guid sessionId, Guid termId, Guid classLevelId, ModerationStatus currentStatus, ModerationStatus newStatus, CancellationToken cancellationToken);
}
