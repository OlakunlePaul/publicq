using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Application.Models.Assignment;
using PublicQ.Infrastructure.Options;
using PublicQ.Infrastructure.Persistence;
using PublicQ.Infrastructure.Persistence.Entities.Assignment;
using PublicQ.Shared;

namespace PublicQ.Infrastructure.Services;

/// <summary>
/// Implementation of the assignment service.
/// <seealso cref="IAssignmentService"/>
/// </summary>
public class AssignmentService(
    ApplicationDbContext dbContext,
    IOptionsMonitor<AssignmentServiceOptions> options,
    ILogger<AssignmentService> logger) : IAssignmentService
{
    /// <inheritdoc cref="IAssignmentService.CreateAsync"/>
    public async Task<Response<AssignmentDto, GenericOperationStatuses>> CreateAsync(
        string createdByUserId,
        AssignmentCreateDto assignmentCreateDto,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Create new assignment request received");
        Guard.AgainstNull(assignmentCreateDto, nameof(assignmentCreateDto));
        Guard.AgainstNullOrWhiteSpace(createdByUserId, nameof(createdByUserId));

        var assignmentWithSameTitleExists = await dbContext.Assignments
            .AnyAsync(a => a.NormalizedTitle == assignmentCreateDto.Title.ToUpperInvariant(), cancellationToken);
        if (assignmentWithSameTitleExists)
        {
            logger.LogWarning("Create assignment failed. Assignment with title {Title} already exists",
                assignmentCreateDto.Title);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict,
                $"Assignment with title '{assignmentCreateDto.Title}' already exists");
        }

        var groupExistWithAtLeastOneMember = await dbContext.Groups
            .AnyAsync(g => g.Id == assignmentCreateDto.GroupId && 
                             g.GroupMemberEntities.Count != 0, cancellationToken);
        
        if (!groupExistWithAtLeastOneMember)
        {
            logger.LogWarning("Create assignment failed. No group found with id {GroupId} or it has no members",
                assignmentCreateDto.GroupId);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.BadRequest,
                $"Group with id '{assignmentCreateDto.GroupId}' not found or it has no members");
        }
        
        if (assignmentCreateDto.StudentIds.Count > 0)
        {
            var validationResponse = await GetValidatedStudentsAsync(assignmentCreateDto.StudentIds, cancellationToken);
            if (validationResponse.IsFailed)
            {
                return Response<AssignmentDto, GenericOperationStatuses>.Failure(
                    validationResponse.Status,
                    validationResponse.Message,
                    validationResponse.Errors);
            }
        }

        // Start transaction for atomic operation
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var nowUtc = DateTime.UtcNow;
        try
        {
            var assignmentToCreate = new AssignmentEntity
            {
                GroupId = assignmentCreateDto.GroupId,
                Title = assignmentCreateDto.Title,
                NormalizedTitle = assignmentCreateDto.Title.ToUpperInvariant(),
                Description = assignmentCreateDto.Description,
                IsPublished = false,
                StartDateUtc = assignmentCreateDto.StartDateUtc,
                EndDateUtc = assignmentCreateDto.EndDateUtc,
                CreatedAtUtc = nowUtc,
                CreatedByUser = createdByUserId,
                RandomizeQuestions = assignmentCreateDto.RandomizeQuestions,
                RandomizeAnswers = assignmentCreateDto.RandomizeAnswers,
                ShowResultsImmediately = assignmentCreateDto.ShowResultsImmediately,
                SubjectId = assignmentCreateDto.SubjectId,
                ClassLevelId = assignmentCreateDto.ClassLevelId,
                MaxTabSwitches = assignmentCreateDto.MaxTabSwitches
            };

            logger.LogDebug("Adding new assignment to the database: {@AssignmentEntity}", assignmentToCreate);
            logger.LogInformation(
                "Adding new assignment to the database for {GroupId} group id. Created by {CreatedBy}",
                assignmentToCreate.GroupId,
                assignmentToCreate.CreatedByUser);

            var response = await dbContext.Assignments
                .AddAsync(assignmentToCreate, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("New assignment added to the database with id {AssignmentId}", response.Entity.Id);

            // Create student assignments within the same transaction
            var studentAssignmentsToCreate = assignmentCreateDto
                .StudentIds
                .Select(id => new StudentAssignmentEntity
                {
                    AssignmentId = response.Entity.Id,
                    StudentId = id,
                }).ToList();

            if (studentAssignmentsToCreate.Count > 0)
            {
                logger.LogDebug("Adding {Count} student assignments to the database",
                    studentAssignmentsToCreate.Count);

                await dbContext.StudentAssignments.AddRangeAsync(studentAssignmentsToCreate, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Added {Count} student assignments to the database",
                    studentAssignmentsToCreate.Count);
            }
            else
            {
                logger.LogDebug("No student assignments to add to the database");
            }

            // Commit transaction - all operations succeeded
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Assignment creation transaction completed successfully for assignment {AssignmentId}",
                response.Entity.Id);

            return Response<AssignmentDto, GenericOperationStatuses>.Success(
                response.Entity.ConvertToDto(),
                GenericOperationStatuses.Completed,
                "Assignment created successfully");
        }
        catch (Exception ex)
        {
            // Rollback transaction on any error
            await transaction.RollbackAsync(cancellationToken);

            logger.LogError(ex, "Failed to create assignment. Transaction rolled back.");

            return Response<AssignmentDto, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                "Failed to create assignment");
        }
    }
    
    /// <inheritdoc cref="IAssignmentService.GetByIdAsync"/>
    public async Task<Response<AssignmentDto, GenericOperationStatuses>> GetByIdAsync(Guid assignmentId,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Get assignment request received for {AssignmentId}", assignmentId);

        var assignment = await dbContext.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == assignmentId, cancellationToken);

        if (assignment is null)
        {
            logger.LogWarning("Get assignment failed. No assignment found with id {AssignmentId}", assignmentId);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"Assignment with id '{assignmentId}' not found");
        }

        logger.LogInformation("Assignment with id {AssignmentId} retrieved successfully", assignmentId);

        return Response<AssignmentDto, GenericOperationStatuses>.Success(
            assignment.ConvertToDto(),
            GenericOperationStatuses.Completed,
            "Assignment retrieved successfully");
    }

    /// <inheritdoc cref="IAssignmentService.UpdateAsync"/>
    public async Task<Response<AssignmentDto, GenericOperationStatuses>> UpdateAsync(
        AssignmentUpdateDto updateDto,
        string updatedByUser,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Update assignment request received");
        Guard.AgainstNull(updateDto, nameof(updateDto));
        Guard.AgainstNullOrWhiteSpace(updatedByUser, nameof(updatedByUser));

        var assignmentToUpdate = await dbContext.Assignments
            .FindAsync([updateDto.Id], cancellationToken);

        if (assignmentToUpdate is null)
        {
            logger.LogWarning("Update failed. No assignment found with id {AssignmentId}", updateDto.Id);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"Assignment with id '{updateDto.Id}' not found");
        }

        if (!string.Equals(updateDto.Title.ToUpperInvariant(),
                assignmentToUpdate.NormalizedTitle,
                StringComparison.InvariantCultureIgnoreCase))
        {
            var assignmentWithSameTitleExists = await dbContext.Assignments
                .AnyAsync(a => a.NormalizedTitle == updateDto.Title.ToUpperInvariant(), cancellationToken);
            if (assignmentWithSameTitleExists)
            {
                logger.LogWarning("Create assignment failed. Assignment with title {Title} already exists",
                    updateDto.Title);
                return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict,
                    $"Assignment with title '{updateDto.Title}' already exists");
            }
        }

        assignmentToUpdate.Title = updateDto.Title;
        assignmentToUpdate.Description = updateDto.Description;
        assignmentToUpdate.StartDateUtc = updateDto.StartDateUtc;
        assignmentToUpdate.EndDateUtc = updateDto.EndDateUtc;
        assignmentToUpdate.RandomizeQuestions = updateDto.RandomizeQuestions;
        assignmentToUpdate.RandomizeAnswers = updateDto.RandomizeAnswers;
        assignmentToUpdate.ShowResultsImmediately = updateDto.ShowResultsImmediately;
        assignmentToUpdate.SubjectId = updateDto.SubjectId;
        assignmentToUpdate.ClassLevelId = updateDto.ClassLevelId;
        assignmentToUpdate.MaxTabSwitches = updateDto.MaxTabSwitches;
        assignmentToUpdate.UpdatedAtUtc = DateTime.UtcNow;
        assignmentToUpdate.UpdatedByUser = updatedByUser;

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Assignment with id {AssignmentId} successfully updated", updateDto.Id);
        
        var updatedAssignment = await dbContext.Assignments
            .AsNoTracking()
            .Include(x => x.StudentAssignments)
            .Include(x => x.Group)
            .FirstOrDefaultAsync(a => a.Id == updateDto.Id, cancellationToken);

        if (updatedAssignment is null)
        {
            // This should not happen as we already checked existence
            logger.LogError("Unexpected error: Assignment with id {AssignmentId} not found after update", updateDto.Id);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.Failed,
                $"Unexpected error: Assignment with id '{updateDto.Id}' not found after update");
        }
        
        return Response<AssignmentDto, GenericOperationStatuses>.Success(
            updatedAssignment.ConvertToDto(),
            GenericOperationStatuses.Completed,
            $"Assignment with ID '{assignmentToUpdate.Id}' updated successfully");
    }

    /// <inheritdoc cref="IAssignmentService.AddStudentsAsync"/>
    public async Task<Response<AssignmentDto, GenericOperationStatuses>> AddStudentsAsync(
        Guid assignmentId,
        HashSet<string> studentIds,
        string updatedByUser,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Adding students request received");
        Guard.AgainstNullOrWhiteSpace(updatedByUser, nameof(updatedByUser));
        
        if (studentIds.Count == 0)
        {
            logger.LogWarning("No students provided to add to assignment with id {AssignmentId}", assignmentId);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.BadRequest,
                "No students provided to add");
        }
        
        var assignment = await dbContext.Assignments
            .FindAsync([assignmentId], cancellationToken);
        
        if (assignment is null)
        {
            logger.LogWarning("Add students failed. No assignment found with id {AssignmentId}", assignmentId);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"Assignment with id '{assignmentId}' not found");
        }

        var existingAssignments = await dbContext.StudentAssignments
            .LongCountAsync(eta => eta.AssignmentId == assignmentId && studentIds.Contains(eta.StudentId),
                cancellationToken);

        if (existingAssignments > 0)
        {
            logger.LogWarning("Some students are already assigned to assignment with id {AssignmentId}",
                assignmentId);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict,
                "One or more students are already assigned to this assignment");
        }

        var validatedStudentsResponse = await GetValidatedStudentsAsync(studentIds, cancellationToken);
        if (validatedStudentsResponse.IsFailed)
        {
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(
                validatedStudentsResponse.Status,
                validatedStudentsResponse.Message,
                validatedStudentsResponse.Errors);
        }

        var assignmentsToAdd = validatedStudentsResponse.Data!
            .Select(et => new StudentAssignmentEntity
            {
                AssignmentId = assignmentId,
                StudentDisplayName = $"{et.FullName}" + 
                                       $"{(string.IsNullOrWhiteSpace(et.Email) ? string.Empty : $" ({et.Email})")}",
                StudentId = et.Id
            }).ToList();

        logger.LogInformation("Adding {Count} students to assignment with id {AssignmentId}",
            assignmentsToAdd.Count, assignmentId);
        
        assignment.UpdatedAtUtc = DateTime.UtcNow;
        assignment.UpdatedByUser = updatedByUser;
        
        await dbContext.StudentAssignments.AddRangeAsync(assignmentsToAdd, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Assigned {Count} students to assignment with id {AssignmentId}",
            assignmentsToAdd.Count,
            assignmentId);
        
        var updatedAssignment = await dbContext.Assignments
            .AsNoTracking()
            .Include(a => a.StudentAssignments)
            .Include(a => a.Group)
            .FirstOrDefaultAsync(a => a.Id == assignmentId, cancellationToken);

        if (updatedAssignment is null)
        {
            // This should not happen as we already checked existence
            logger.LogError("Unexpected error: Assignment with id {AssignmentId} not found after adding students",
                assignmentId);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.Failed,
                $"Unexpected error: Assignment with id '{assignmentId}' not found after adding students");
        }

        return Response<AssignmentDto, GenericOperationStatuses>.Success(
            updatedAssignment.ConvertToDto(),
            GenericOperationStatuses.Completed,
            $"Added {assignmentsToAdd.Count} students to assignment with id '{assignmentId}' successfully");
    }

    /// <inheritdoc cref="IAssignmentService.GetStudentsAsync"/>
    public async Task<Response<IList<User>, GenericOperationStatuses>> GetStudentsAsync(
        Guid assignmentId, 
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Get students for assignment request received");
        
        var assignment = await dbContext.Assignments
            .FindAsync([assignmentId], cancellationToken);

        if (assignment is null)
        {
            logger.LogWarning("Get students failed. No assignment found with id {AssignmentId}", assignmentId);
            return Response<IList<User>, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"Assignment with id '{assignmentId}' not found");
        }
        
        var studentIds = await dbContext.StudentAssignments
            .AsNoTracking()
            .Where(eta => eta.AssignmentId == assignmentId)
            .Select(eta => eta.StudentId)
            .ToHashSetAsync(cancellationToken);

        if (studentIds.Count == 0)
        {
            logger.LogDebug("No students assigned to assignment with id {AssignmentId}", assignmentId);
            return Response<IList<User>, GenericOperationStatuses>.Success(
                new List<User>(),
                GenericOperationStatuses.Completed,
                "No students assigned to this assignment");
        }
        
        var combinedStudents = await GetStudentsAsync(studentIds, cancellationToken);

        if (combinedStudents.Count == 0)
        {
            return Response<IList<User>, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound,
                "No students found for this assignment");
        }
        
        logger.LogDebug("Found {Count} students for assignment with id {AssignmentId}",
            combinedStudents.Count, assignmentId);
        
        return Response<IList<User>, GenericOperationStatuses>.Success(
            combinedStudents,
            GenericOperationStatuses.Completed,
            $"Found {combinedStudents.Count} students for assignment with id '{assignmentId}'");
    }

    /// <inheritdoc cref="IAssignmentService.RemoveStudentsAsync"/>
    public async Task<Response<AssignmentDto, GenericOperationStatuses>> RemoveStudentsAsync(
        Guid assignmentId,
        HashSet<string> studentIds,
        string updatedByUser,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Removing students request received");
        Guard.AgainstNullOrWhiteSpace(updatedByUser, nameof(updatedByUser));
        
        var assignment = await dbContext.Assignments
            .FindAsync([assignmentId], cancellationToken);

        if (assignment is null)
        {
            logger.LogWarning("Remove students failed. No assignment found with id {AssignmentId}",
                assignmentId);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"Assignment with id '{assignmentId}' not found");
        }

        var assignmentsToRemove = await dbContext.StudentAssignments
            .Where(eta => eta.AssignmentId == assignmentId && studentIds.Contains(eta.StudentId))
            .ToListAsync(cancellationToken);

        if (assignmentsToRemove.Count == studentIds.Count)
        {
            logger.LogWarning("Some students are not assigned to assignment with id {AssignmentId}",
                assignmentId);
        }

        dbContext.StudentAssignments.RemoveRange(assignmentsToRemove);

        logger.LogInformation("Removing {Count} students from assignment with id {AssignmentId}",
            assignmentsToRemove.Count,
            assignmentId);
        
        assignment.UpdatedAtUtc = DateTime.UtcNow;
        assignment.UpdatedByUser = updatedByUser;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Removed {Count} students from assignment with id {AssignmentId}",
            assignmentsToRemove.Count,
            assignmentId);
        
        var updatedAssignment = await dbContext.Assignments
            .AsNoTracking()
            .Include(a => a.StudentAssignments)
            .Include(a => a.Group)
            .FirstOrDefaultAsync(a => a.Id == assignmentId, cancellationToken);
        
        if (updatedAssignment is null)
        {
            // This should not happen as we already checked existence
            logger.LogError("Unexpected error: Assignment with id {AssignmentId} not found after removing students",
                assignmentId);
            return Response<AssignmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.Failed,
                $"Unexpected error: Assignment with id '{assignmentId}' not found after removing students");
        }

        return Response<AssignmentDto, GenericOperationStatuses>.Success(
            updatedAssignment.ConvertToDto(),
            GenericOperationStatuses.Completed,
            $"Removed {assignmentsToRemove.Count} students from assignment with id '{assignmentId}' successfully");
    }

    /// <inheritdoc cref="IAssignmentService.PublishAsync"/>
    public async Task<Response<GenericOperationStatuses>> PublishAsync(
        Guid assignmentId,
        string updatedByUser,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Publishing assignment with {ID} ID request received", assignmentId);
        Guard.AgainstNullOrWhiteSpace(updatedByUser, nameof(updatedByUser));

        var assignmentToPublish = await dbContext.Assignments
            .FindAsync([assignmentId], cancellationToken);

        if (assignmentToPublish is null)
        {
            logger.LogWarning("Publish failed. No assignment found with id {AssignmentId}", assignmentId);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"Assignment with id '{assignmentId}' not found");
        }

        if (assignmentToPublish.IsPublished)
        {
            logger.LogWarning("Unable to publish assignment with id {AssignmentId} as it is already published",
                assignmentId);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict,
                $"Assignment with id '{assignmentId}' is already published");
        }

        logger.LogInformation("Publishing assignment with id {AssignmentId}", assignmentId);
        assignmentToPublish.IsPublished = true;
        assignmentToPublish.UpdatedAtUtc = DateTime.UtcNow;
        assignmentToPublish.UpdatedByUser = updatedByUser;

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Published assignment with id {AssignmentId}", assignmentId);

        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed,
            $"Assignment with id '{assignmentId}' published successfully");
    }

    /// <inheritdoc cref="IAssignmentService.DeleteAsync"/>
    public async Task<Response<GenericOperationStatuses>> DeleteAsync(
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Delete assignment request received for {AssignmentId}", assignmentId);

        var assignmentToRemove = await dbContext.Assignments
            .FindAsync([assignmentId], cancellationToken);

        if (assignmentToRemove is null)
        {
            logger.LogWarning("Delete failed. No assignment found with id {AssignmentId}", assignmentId);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"Assignment with id '{assignmentId}' not found");
        }

        dbContext.Assignments.Remove(assignmentToRemove);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Assignment with id {AssignmentId} successfully removed", assignmentId);

        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed,
            $"Assignment with id '{assignmentId}' deleted successfully");
    }
    

    /// <inheritdoc cref="IAssignmentService.GetAssignmentsAsync"/>
    public async Task<Response<PaginatedResponse<AssignmentDto>, GenericOperationStatuses>> GetAssignmentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? titleFilter = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Get assignments request received.");

        if (pageNumber < 1)
        {
            pageNumber = 1;
            logger.LogInformation("Page number less than 1. Defaulting to 1.");
        }

        if (pageSize < 1 || pageSize > options.CurrentValue.MaxPageSize)
        {
            pageSize = options.CurrentValue.MaxPageSize;
            logger.LogInformation("Page size out of range. Defaulting to {DefaultPageSize}.",
                options.CurrentValue.MaxPageSize);
        }

        logger.LogDebug(
            "Fetching assignments. Query - PageNumber: {PageNumber}, PageSize: {PageSize}, TitleFilter: {TitleFilter}",
            pageNumber,
            pageSize,
            titleFilter);

        var titleEmpty = string.IsNullOrWhiteSpace(titleFilter);
        var query = dbContext.Assignments
            .Where(a => titleEmpty || EF.Functions.Like(a.Title, $"%{titleFilter}%"))
            .AsNoTracking()
            .Include(a => a.StudentAssignments)
            .Include(a => a.Group)
            .AsQueryable();

        var totalCount = await query.LongCountAsync(cancellationToken);

        var paginatedResponse = new PaginatedResponse<AssignmentDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        if (totalCount <= 0)
        {
            logger.LogInformation("No assignments found.");
            return Response<PaginatedResponse<AssignmentDto>, GenericOperationStatuses>.Success(
                paginatedResponse,
                GenericOperationStatuses.NotFound,
                "No assignments found");
        }

        var assignments = await query
            .OrderByDescending(a => a.UpdatedAtUtc ?? a.CreatedAtUtc)
            .ThenByDescending(a => a.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        logger.LogDebug("Fetched {Count} assignments out of {TotalCount}.",
            assignments.Count, totalCount);

        var assignmentDtos = assignments
            .Select(a => a.ConvertToDto())
            .ToList();

        paginatedResponse.Data.AddRange(assignmentDtos);

        return Response<PaginatedResponse<AssignmentDto>, GenericOperationStatuses>.Success(
            paginatedResponse,
            GenericOperationStatuses.Completed,
            "Assignments retrieved successfully");
    }

    /// <inheritdoc cref="IAssignmentService.GetAvailableAssignmentsAsync"/>
    public async Task<Response<IList<StudentAssignmentDto>, GenericOperationStatuses>> GetAvailableAssignmentsAsync(
        string studentId, 
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Get assignments by student ID request received for {StudentId}", studentId);
        Guard.AgainstNullOrWhiteSpace(studentId, nameof(studentId));

        // First, resolve the studentId to the actual internal Id if it's an admission number
        var resolvedId = studentId;
        
        // Check if the provided ID directly matches any Student or User
        var existsAsId = await dbContext.Students.AnyAsync(e => e.Id == studentId, cancellationToken)
                         || await dbContext.Users.AnyAsync(u => u.Id == studentId, cancellationToken);
        
        if (!existsAsId)
        {
            // If not found as ID, check if it's an AdmissionNumber
            var userWithAdmissionNumber = await dbContext.Students
                .AsNoTracking()
                .Where(e => e.AdmissionNumber == studentId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken)
                ?? await dbContext.Users
                .AsNoTracking()
                .Where(u => u.AdmissionNumber == studentId)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);
                
            if (userWithAdmissionNumber != null)
            {
                resolvedId = userWithAdmissionNumber;
                logger.LogInformation("Resolved Admission Number {AdmissionNumber} to StudentId {StudentId}", studentId, resolvedId);
            }
        }

        // Find the student's current class level for the active session
        var activeSession = await dbContext.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.IsActive, cancellationToken);
        Guid? studentClassId = null;
        
        if (activeSession != null)
        {
            studentClassId = await dbContext.StudentAssessments
                .AsNoTracking()
                .Where(a => a.StudentId == resolvedId && a.SessionId == activeSession.Id)
                .Select(a => (Guid?)a.ClassLevelId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var query = dbContext.Assignments
            .AsNoTracking()
            .Where(a => a.IsPublished
                        && (a.StudentAssignments.Any(sa => sa.StudentId == resolvedId)
                            || (studentClassId != null && a.ClassLevelId == studentClassId)));
        
        var totalCount = await query.LongCountAsync(cancellationToken);
        
        if (totalCount == 0)
        {
            logger.LogInformation("No assignments found.");
            return Response<IList<StudentAssignmentDto>, GenericOperationStatuses>.Success(
                new List<StudentAssignmentDto>(),
                GenericOperationStatuses.NotFound,
                "No assignments found");
        }

        var items = await query
            .Include(a => a.Group)
                .ThenInclude(g => g.GroupMemberEntities)
            .Include(a => a.StudentAssignments.Where(sa => sa.StudentId == resolvedId))
                .ThenInclude(sa => sa.ModuleProgress)
            .OrderBy(a => a.StartDateUtc)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var dtos = new List<StudentAssignmentDto>();
        bool previousActiveIncompleteFound = false;

        foreach (var a in items)
        {
            var sa = a.StudentAssignments.FirstOrDefault();
            var studentAssignmentId = sa?.Id ?? Guid.Empty;
            var dto = a.ConvertToStudentAssignmentDto(studentAssignmentId);
            
            // 1. Calculate Completion
            // An assignment is completed if it has a student assignment record AND
            // all modules in its group have corresponding completed module progress records.
            bool isCompleted = false;
            if (sa != null && a.Group?.GroupMemberEntities != null && a.Group.GroupMemberEntities.Count > 0)
            {
                var completedModuleIds = sa.ModuleProgress
                    .Where(mp => mp.CompletedAtUtc != null)
                    .Select(mp => mp.GroupMemberId)
                    .ToHashSet();
                
                isCompleted = a.Group.GroupMemberEntities.All(gme => completedModuleIds.Contains(gme.Id));
            }
            
            dto.IsCompleted = isCompleted;

            // 2. Calculate Automatic Progression Lock
            // An assignment is locked if:
            // - It has not been completed yet
            // - There exists an EARLIER assignment that has started but is NOT yet completed.
            
            // We only block based on assignments that have already "Started" (StartDateUtc <= now).
            // Future assignments are "Scheduled" and locked by time anyway in the UI, 
            // but for logical consistency we follow the sequence.
            
            dto.IsProgressionLocked = previousActiveIncompleteFound;

            // If this assignment has started and is not completed, it blocks all subsequent ones.
            if (a.StartDateUtc <= now && !isCompleted)
            {
                previousActiveIncompleteFound = true;
            }

            dtos.Add(dto);
        }

        logger.LogInformation("Fetched {Count} assignments out of {Total} for student {StudentId}",
            dtos.Count, 
            totalCount, 
            studentId);
        
        return Response<IList<StudentAssignmentDto>, GenericOperationStatuses>.Success(
                dtos, 
                GenericOperationStatuses.Completed,
                "Assignments retrieved successfully");
    }

    /// <inheritdoc cref="IAssignmentService.GetAssignmentCountAsync"/>
    public async Task<Response<long, GenericOperationStatuses>> GetAssignmentCountAsync(
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Get assignment count request received.");

        var count = await dbContext.Assignments
            .AsNoTracking()
            .LongCountAsync(cancellationToken);
        logger.LogDebug("Total assignments count: {Count}", count);

        return Response<long, GenericOperationStatuses>.Success(
            count,
            GenericOperationStatuses.Completed,
            $"Total assignments count: '{count}' found.");
    }

    /// <inheritdoc cref="IAssignmentService.RecordTabSwitchAsync"/>
    public async Task<Response<GenericOperationStatuses>> RecordTabSwitchAsync(
        Guid assignmentId,
        string studentId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Recording tab switch for assignment {AssignmentId}, student {StudentId}", assignmentId, studentId);

        var studentAssignment = await dbContext.StudentAssignments
            .Include(eta => eta.Assignment)
            .FirstOrDefaultAsync(eta => eta.AssignmentId == assignmentId && eta.StudentId == studentId, cancellationToken);

        if (studentAssignment == null)
        {
            logger.LogWarning("Record tab switch failed. No assignment tracking found for student {StudentId} in assignment {AssignmentId}", studentId, assignmentId);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assignment tracking record not found.");
        }

        studentAssignment.TabSwitchCount++;
        studentAssignment.LastTabSwitchAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        var maxAllowed = studentAssignment.Assignment.MaxTabSwitches;
        if (maxAllowed > 0 && studentAssignment.TabSwitchCount >= maxAllowed)
        {
            logger.LogWarning("Locking student {StudentId} for assignment {AssignmentId}. Tab switches: {Count}/{Max}",
                studentId, studentAssignment.TabSwitchCount, maxAllowed, assignmentId);
            
            studentAssignment.IsLocked = true;
            await dbContext.SaveChangesAsync(cancellationToken);

            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict, 
                $"LOCKED: Tab switch limit exceeded ({studentAssignment.TabSwitchCount}/{maxAllowed}). Exam has been locked.");
        }

        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Tab switch recorded.");
    }

    /// <inheritdoc cref="IAssignmentService.UnlockAssignmentAsync"/>
    public async Task<Response<GenericOperationStatuses>> UnlockAssignmentAsync(
        Guid assignmentId,
        string studentId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Unlocking assignment {AssignmentId} for student {StudentId}", assignmentId, studentId);

        var studentAssignment = await dbContext.StudentAssignments
            .FirstOrDefaultAsync(eta => eta.AssignmentId == assignmentId && eta.StudentId == studentId, cancellationToken);

        if (studentAssignment == null)
        {
            logger.LogWarning("Unlock failed. No assignment tracking found for student {StudentId} in assignment {AssignmentId}", studentId, assignmentId);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assignment tracking record not found.");
        }

        studentAssignment.TabSwitchCount = 0;
        studentAssignment.IsLocked = false;
        studentAssignment.LastTabSwitchAtUtc = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully unlocked student {StudentId} in assignment {AssignmentId}", studentId, assignmentId);

        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Student session unlocked successfully.");
    }
   
    /// <summary>
    /// This service method validates that all provided student IDs exist in the system.
    /// </summary>
    /// <param name="userIds">Array of user IDs.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns a list of <see cref="User"/> wrapped into <see cref="Response{TData, TStatus}"/></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task<Response<IList<User>, GenericOperationStatuses>> GetValidatedStudentsAsync(
        HashSet<string> userIds, 
        CancellationToken cancellationToken)
    {
        var students = await GetStudentsAsync(userIds, cancellationToken);
        if (students.Count == 0)
        {
            return Response<IList<User>, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound,
                "No students found for the provided IDs");
        }

        var foundUserIds = students.Select(u => u.Id).ToHashSet();
        var missingUserIds = userIds.Except(foundUserIds, StringComparer.InvariantCultureIgnoreCase).ToList();

        if (missingUserIds.Count > 0)
        {
            logger.LogWarning("Validation failed. Some students not found: {MissingUserIds}",
                string.Join(", ", missingUserIds));
            return Response<IList<User>, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"Some students not found: {string.Join(", ", missingUserIds)}");
        }

        logger.LogDebug("All provided students validated successfully.");
        return Response<IList<User>, GenericOperationStatuses>.Success(
            students, 
            GenericOperationStatuses.Completed,
            "All students validated successfully");
    }
    
    // TODO: Create user repository to avoid direct DbContext access in the service
    // Otherwise, we have circular dependency between UserService and AssignmentService
    /// <summary>
    /// This method retrieves and combines exam takers from both ExamTakers and Users tables based on provided IDs.
    /// </summary>
    /// <param name="examTakerIds">Exam taker IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns an array of <see cref="User"/></returns>
    private async Task<List<User>> GetStudentsAsync(HashSet<string> studentIds, CancellationToken cancellationToken)
    {
        var studentIdsUppercase = studentIds.Select(id => id.ToUpperInvariant()).ToHashSet();
        var students = await dbContext.Students
            .AsNoTracking()
            .Where(e => studentIdsUppercase.Contains(e.Id))
            .Select(e => e.ConvertToUser())
            .ToListAsync(cancellationToken);

        var users = await dbContext.Users
            .AsNoTracking()
            .Where(e => studentIds.Contains(e.Id))
            .Select(e => e.ConvertToUser())
            .ToListAsync(cancellationToken);
        
        var combinedStudents = students
            .Concat(users)
            .ToList();
        
        return combinedStudents;
    }
}