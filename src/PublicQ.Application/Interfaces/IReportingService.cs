using PublicQ.Application.Models;
using PublicQ.Application.Models.Assignment;
using PublicQ.Application.Models.Reporting;

namespace PublicQ.Application.Interfaces;

/// <summary>
/// This interface defines the contract for reporting services within the application.
/// </summary>
public interface IReportingService
{
    /// <summary>
    /// Gets paginated assignment summary reports.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-based indexing).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A response containing paginated assignment summary report data.</returns>
    [Cacheable]
    Task<Response<PaginatedResponse<AssignmentSummaryReportDto>, GenericOperationStatuses>> GetAllAssignmentSummaryReportAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves a comprehensive report for a specific assignment, including detailed student progress and performance data.
    /// </summary>
    /// <param name="assignmentId">The unique identifier of the assignment to generate the report for.</param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>A response containing the full assignment report data including student details, progress, and performance metrics.</returns>
    [Cacheable]
    Task<Response<AssignmentReportDto, GenericOperationStatuses>> GetAssignmentFullReportAsync(
        Guid assignmentId,
        CancellationToken cancellation);
    
    /// <summary>
    /// Retrieves a summary report for a specific assignment with high-level statistics and overview data.
    /// </summary>
    /// <param name="assignmentId">The unique identifier of the assignment to generate the summary report for.</param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>A response containing the assignment summary report with aggregate statistics.</returns>
    [Cacheable]
    Task<Response<AssignmentSummaryReportDto, GenericOperationStatuses>> GetAssignmentSummaryReportAsync(
        Guid assignmentId,
        CancellationToken cancellation);
    
    /// <summary>
    /// Retrieves detailed reports for multiple students, optionally filtered by a specific assignment.
    /// </summary>
    /// <param name="studentIds">The collection of student identifiers to generate reports for.</param>
    /// <param name="assignmentId">Optional assignment identifier to filter reports by. If null, returns reports across all assignments.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A response containing a list of student reports with their performance and progress data.</returns>
    [Cacheable]
    Task<Response<IList<StudentReportDto>, GenericOperationStatuses>> GetStudentReportsAsync(
        IList<string> studentIds,
        Guid? assignmentId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a detailed report for a single student, optionally filtered by a specific assignment.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student to generate the report for.</param>
    /// <param name="assignmentId">Optional assignment identifier to filter the report by. If null, returns data across all assignments.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A response containing the student report with performance metrics, progress data, and assignment details.</returns>
    [Cacheable]
    Task<Response<StudentReportDto, GenericOperationStatuses>> GetStudentReportAsync(
        string studentId,
        Guid? assignmentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated students.
    /// </summary>
    /// <param name="idFilter">Filter on student ID</param>
    /// <param name="nameFilter">Filter on student display name</param>
    /// <param name="pageNumber">The page number to retrieve (1-based indexing).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A response containing paginated assignment summary report data.</returns>
    [Cacheable]
    Task<Response<PaginatedResponse<IndividualStudentReportDto>, GenericOperationStatuses>> GetAllStudentsAsync(
        string? idFilter,
        string? nameFilter,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}