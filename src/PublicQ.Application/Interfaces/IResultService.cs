using PublicQ.Application.Models;
using PublicQ.Application.Models.Academic;

namespace PublicQ.Application.Interfaces;

/// <summary>
/// Service for managing academic results and report cards.
/// </summary>
public interface IResultService
{
    /// <summary>
    /// Uploads and parses a result CSV file (Mercy's Gate format).
    /// </summary>
    Task<ResultUploadResponse> UploadResultCsvAsync(Stream fileStream, Guid sessionId, Guid termId, Guid classLevelId);

    /// <summary>
    /// Gets a detailed assessment record for report card generation.
    /// </summary>
    Task<StudentAssessmentDto?> GetStudentAssessmentAsync(Guid assessmentId);

    /// <summary>
    /// Gets all assessments for a specific term and class.
    /// </summary>
    Task<IEnumerable<StudentAssessmentDto>> GetClassAssessmentsAsync(Guid sessionId, Guid termId, Guid classLevelId);

    /// <summary>
    /// Gets results for all children of a parent.
    /// </summary>
    Task<IEnumerable<StudentAssessmentDto>> GetParentChildrenResultsAsync(string parentUserId);

    /// <summary>
    /// Updates the moderation status of an assessment.
    /// </summary>
    Task UpdateStatusAsync(Guid assessmentId, ModerationStatus status);
}
