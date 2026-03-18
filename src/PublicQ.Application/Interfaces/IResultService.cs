using PublicQ.Application.Models;
using PublicQ.Domain.Enums;

namespace PublicQ.Application.Interfaces;

/// <summary>
/// Service to manage student academic results, moderation, and calculation.
/// </summary>
public interface IResultService
{
    /// <summary>
    /// Calculates and saves individual subject totals, overall average, and ranks students (position) within a class.
    /// This should typically be called before moving results to 'Moderated' or 'Approved' status.
    /// </summary>
    /// <param name="sessionId">The academic session ID</param>
    /// <param name="termId">The term ID</param>
    /// <param name="classLevelId">The class level ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns a generic operation status</returns>
    Task<Response<GenericOperationStatuses>> CalculateClassResultsAsync(
        Guid sessionId, 
        Guid termId, 
        Guid classLevelId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the moderation status of a specific student assessment.
    /// </summary>
    /// <param name="assessmentId">The specific assessment ID</param>
    /// <param name="newStatus">The new moderation status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns a generic operation status</returns>
    Task<Response<GenericOperationStatuses>> UpdateAssessmentStatusAsync(
        Guid assessmentId, 
        ModerationStatus newStatus, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the non-academic details of a student's assessment (Affective, Psychomotor, Attendance, Remarks).
    /// </summary>
    Task<Response<GenericOperationStatuses>> UpdateAssessmentDetailsAsync(
        Guid assessmentId, 
        UpdateAssessmentDetailsDto details, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Locks or unlocks an assessment, preventing parents from viewing it if locked.
    /// </summary>
    Task<Response<GenericOperationStatuses>> ToggleAssessmentLockAsync(
        Guid assessmentId, 
        bool isLocked, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch updates the moderation status for an entire class.
    /// Useful for an Admin approving all "Moderated" results to "Published".
    /// </summary>
    Task<Response<GenericOperationStatuses>> BatchUpdateClassStatusAsync(
        Guid sessionId, 
        Guid termId, 
        Guid classLevelId, 
        ModerationStatus currentStatus,
        ModerationStatus newStatus, 
        CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Retrieves all assessments for a specific class in a specific term/session.
    /// Used for populating the bulk entry grid.
    /// </summary>
    Task<Response<IList<AssessmentReportDto>, GenericOperationStatuses>> GetClassAssessmentsAsync(
        Guid sessionId, 
        Guid termId, 
        Guid classLevelId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves full details of a specific assessment including traits and subject scores.
    /// </summary>
    Task<Response<AssessmentDetailsDto, GenericOperationStatuses>> GetAssessmentDetailsAsync(
        Guid assessmentId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves bulk subject scores entered by a teacher.
    /// </summary>
    Task<Response<GenericOperationStatuses>> SaveBulkScoresAsync(
        BulkScoreEntryDto dto, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Helper method to define the grading scale configuration.
    /// E.g. 70-100 = A, 60-69 = B.
    /// In a fully dynamic system, this would be fetched from the database attached to a Template.
    /// </summary>
    string CalculateGrade(decimal score);
}
