using PublicQ.Application.Models;
using PublicQ.Application.Models.Assignment;

namespace PublicQ.Application.Interfaces;

/// <summary>
/// Defines operations for managing assignments and their students.
/// </summary>
public interface IAssignmentService
{
    /// <summary>
    /// Gets an assignment by its ID with full details.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the assignment details and operation status.</returns>
    Task<Response<AssignmentDto, GenericOperationStatuses>> GetByIdAsync(
        Guid assignmentId, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates a new assignment.
    /// </summary>
    /// <param name="createdByUserId">The ID of the user creating the assignment.</param>
    /// <param name="assignmentCreateDto">The assignment creation data transfer object.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the created assignment and operation status.</returns>
    Task<Response<AssignmentDto, GenericOperationStatuses>> CreateAsync(
        string createdByUserId,
        AssignmentCreateDto assignmentCreateDto,
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing assignment.
    /// </summary>
    /// <param name="updateDto">The assignment update data transfer object.</param>
    /// <param name="updatedByUser">Updared by username</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the updated assignment and operation status.</returns>
    Task<Response<AssignmentDto, GenericOperationStatuses>> UpdateAsync(
        AssignmentUpdateDto updateDto,
        string updatedByUser,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds students to an assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <param name="studentIds">The set of student IDs to add.</param>
    /// <param name="updatedByUser">Updated by username</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the updated assignment and operation status.</returns>
    Task<Response<AssignmentDto, GenericOperationStatuses>> AddStudentsAsync(
        Guid assignmentId,
        HashSet<string> studentIds,
        string updatedByUser,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves students for a given assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the list of students and operation status.</returns>
    Task<Response<IList<User>, GenericOperationStatuses>> GetStudentsAsync(
        Guid assignmentId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Removes students from an assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <param name="studentIds">The set of student IDs to remove.</param>
    /// <param name="updatedByUser">Updated by username</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the updated assignment and operation status.</returns>
    Task<Response<AssignmentDto, GenericOperationStatuses>> RemoveStudentsAsync(
        Guid assignmentId,
        HashSet<string> studentIds,
        string updatedByUser,
        CancellationToken cancellationToken);

    /// <summary>
    /// Publishes an assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <param name="updatedByUser">The ID of the user publishing the assignment.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the operation status.</returns>
    Task<Response<GenericOperationStatuses>> PublishAsync(
        Guid assignmentId,
        string updatedByUser,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes an assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the operation status.</returns>
    Task<Response<GenericOperationStatuses>> DeleteAsync(
        Guid assignmentId, 
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a paginated list of assignments with optional filtering.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="titleFilter">Optional filter by assignment title.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing a paginated list of assignments and operation status.</returns>
    Task<Response<PaginatedResponse<AssignmentDto>, GenericOperationStatuses>> GetAssignmentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? titleFilter = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a list of available assignments for a specific student.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing a list of assignments and operation status.</returns>
    Task<Response<IList<StudentAssignmentDto>, GenericOperationStatuses>> GetAvailableAssignmentsAsync(
        string studentId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total number of assignments.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the total count of assignments and operation status.</returns>
    Task<Response<long, GenericOperationStatuses>> GetAssignmentCountAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a browser tab switch or focus loss during an exam.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <param name="examTakerId">The ID of the student.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the operation status.</returns>
    Task<Response<GenericOperationStatuses>> RecordTabSwitchAsync(
        Guid assignmentId,
        string studentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlocks a student's assignment session and resets their tab switch count.
    /// </summary>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A response containing the operation status.</returns>
    Task<Response<GenericOperationStatuses>> UnlockAssignmentAsync(
        Guid assignmentId,
        string studentId,
        CancellationToken cancellationToken = default);
}