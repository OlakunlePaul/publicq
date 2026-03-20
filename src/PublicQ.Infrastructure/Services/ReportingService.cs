using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Application.Models.Assignment;
using PublicQ.Application.Models.Reporting;
using PublicQ.Application.Models.Session;
using PublicQ.Infrastructure.Options;
using PublicQ.Infrastructure.Persistence;
using PublicQ.Infrastructure.Persistence.Entities.Assignment;
using PublicQ.Shared;

namespace PublicQ.Infrastructure.Services;

/// <inheritdoc cref="IReportingService"/>
public class ReportingService(
    ApplicationDbContext dbContext, 
    IOptionsMonitor<ReportingServiceOptions> options,
    ILogger<ReportingService> logger) : IReportingService
{
    /// <inheritdoc cref="IReportingService.GetAllAssignmentSummaryReportAsync"/>
    public async Task<Response<PaginatedResponse<AssignmentSummaryReportDto>, GenericOperationStatuses>>
        GetAllAssignmentSummaryReportAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
    {
        if (pageNumber < 1)
        {
            logger.LogInformation("Page number {PageNumber} is less than 1. Defaulting to 1", pageNumber);
            pageNumber = 1;
        }

        if (pageSize > options.CurrentValue.MaxPageSize)
        {
            logger.LogInformation("Page size {PageSize} exceeds max of {MaxPageSize}. Capping to max.",
                pageSize,
                options.CurrentValue.MaxPageSize);
            pageSize = options.CurrentValue.MaxPageSize;
        }
        
        logger.LogDebug("GetAllAssignmentSummaryReportAsync request received");

        var query = dbContext.Assignments
            .AsNoTracking()
            .AsQueryable();
        
        var assignments = await query
            .OrderByDescending(assignment => assignment.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (assignments.Count == 0)
        {
            logger.LogInformation("No assignments found in the database.");
            return Response<PaginatedResponse<AssignmentSummaryReportDto>, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound,
                "No assignments found.");
        }
        

        var assignmentReports = new List<AssignmentSummaryReportDto>();
        var totalCount = await query.LongCountAsync(cancellationToken);
        var paginatedResponse = new PaginatedResponse<AssignmentSummaryReportDto>
        {
            Data = assignmentReports,
            TotalCount = totalCount,
            PageSize = assignments.Count,
        };
        
        foreach (var assignment in assignments)
        {
            logger.LogDebug("Processing AssignmentId: {AssignmentId}", assignment.Id);
            var assignmentReport = await GetAssignmentSummaryReportAsync(assignment.Id, cancellationToken);

            if (assignmentReport.IsSuccess)
            {
                assignmentReports.Add(assignmentReport.Data!);
            }
            else
            {
                logger.LogWarning("Failed to generate report for AssignmentId: {AssignmentId}. Errors: {Errors}",
                    assignment.Id,
                    string.Join(", ", assignmentReport.Errors));
            }
        }
        
        logger.LogDebug("Assembled {Count} assignment reports", assignmentReports.Count);
        
        return Response<PaginatedResponse<AssignmentSummaryReportDto>, GenericOperationStatuses>.Success(
            paginatedResponse, 
            GenericOperationStatuses.Completed,
            "Assignment summary reports retrieved successfully.");
    }

    /// <inheritdoc cref="IReportingService.GetAssignmentFullReportAsync"/>
    public async Task<Response<AssignmentReportDto, GenericOperationStatuses>> GetAssignmentFullReportAsync(
            Guid assignmentId,
            CancellationToken cancellation)
    {
        logger.LogDebug("GetStudentsAssignmentReportAsync request received for AssignmentId: {AssignmentId}",
            assignmentId);
        
        var assignment = await dbContext.Assignments
            .AsNoTracking()
            .Include(assignmentEntity => assignmentEntity.StudentAssignments)
            .FirstOrDefaultAsync(a => a.Id == assignmentId, cancellation);

        if (assignment == null)
        {
            logger.LogWarning("Assignment with ID {AssignmentId} not found.", assignmentId);
            return Response<AssignmentReportDto, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound,
                $"Assignment with ID {assignmentId} not found.");
        }
        
        // Get all student IDs for batch processing
        var studentIds = assignment.StudentAssignments.Select(eta => eta.StudentId).ToList();
        
        var studentReportsResponse = await GetStudentReportsAsync(
            studentIds, 
            assignmentId, 
            cancellation);

        if (studentReportsResponse.IsFailed)
        {
            logger.LogError("Failed to get reports for assignment {AssignmentId}. Errors: {Errors}",
                assignmentId,
                string.Join(", ", studentReportsResponse.Errors));
            return Response<AssignmentReportDto, GenericOperationStatuses>.Failure(
                studentReportsResponse.Status,
                studentReportsResponse.Message,
                studentReportsResponse.Errors);
        }

        var fullAssignmentReport = AssignmentReportDto.CreateReportFromData(
            assignment.ConvertToDto(),
            studentReportsResponse.Data!);
        
        return Response<AssignmentReportDto, GenericOperationStatuses>.Success(
            fullAssignmentReport,
            GenericOperationStatuses.Completed,
            $"Student assignment reports retrieved successfully for AssignmentId: {assignmentId}");
    }

    /// <inheritdoc cref="IReportingService.GetAssignmentSummaryReportAsync"/>
    public async Task<Response<AssignmentSummaryReportDto, GenericOperationStatuses>> GetAssignmentSummaryReportAsync(
        Guid assignmentId,
        CancellationToken cancellation)
    {
        logger.LogDebug("GetAssignmentSummaryReportAsync request received for AssignmentId: {AssignmentId}",
            assignmentId);

        var assignment = await dbContext.Assignments
            .Include(a => a.StudentAssignments)
                .ThenInclude(eta => eta.ModuleProgress)
                    .ThenInclude(mp => mp.AssessmentModuleVersion)
            .Include(a => a.StudentAssignments)
                .ThenInclude(eta => eta.ModuleProgress)
                    .ThenInclude(mp => mp.QuestionResponses)
            .Include(a => a.Group!)
                .ThenInclude(g => g.GroupMemberEntities)
                    .ThenInclude(gm => gm.AssessmentModule)
                        .ThenInclude(am => am.Versions)
                            .ThenInclude(v => v.Questions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == assignmentId, cancellation);

        if (assignment == null)
        {
            logger.LogWarning("Assignment with ID {AssignmentId} not found.", assignmentId);
            return Response<AssignmentSummaryReportDto, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound,
                $"Assignment with ID {assignmentId} not found.");
        }

        logger.LogDebug("Assembling report data for AssignmentId: {AssignmentId}", assignmentId);

        // Define the completion condition first
        var isModuleCompleted = (ModuleProgressEntity mp) => mp.CompletedAtUtc != null ||
                                                             (mp.StartedAtUtc != null &&
                                                              mp.StartedAtUtc.Value.AddMinutes(mp.DurationInMinutes) <
                                                              DateTime.UtcNow);

        // Get completed assignments (students who completed ALL modules)
        var completedAssignments = assignment.StudentAssignments
            .Where(eta => eta.ModuleProgress.Count > 0 && 
                          assignment.Group != null && 
                          eta.ModuleProgress.Count(isModuleCompleted) == assignment.Group.GroupMemberEntities.Count)
            .ToList();

        // Get in-progress assignments  
        var inProgressAssignments = assignment.StudentAssignments
            .Where(eta => eta.ModuleProgress.Any(mp => mp.StartedAtUtc != null) &&
                          assignment.Group != null &&
                          eta.ModuleProgress.Count(isModuleCompleted) != assignment.Group.GroupMemberEntities.Count)
            .ToList();

        // Get not-started assignments
        var notStartedAssignments = assignment.StudentAssignments
            .Where(eta => !eta.ModuleProgress.Any()) // No progress records at all
            .ToList();

        // Calculate average score across all completed modules
        var averageScore = assignment.StudentAssignments
            .SelectMany(eta => eta.ModuleProgress)
            .Where(isModuleCompleted)
            .Average(mp => (double?)mp.ScorePercentage) ?? 0;

        // Build module reports (grouped by module)
        // Get all modules in the assignment through Group -> GroupMembers -> AssessmentModules
        var moduleReports = assignment.Group!.GroupMemberEntities
            .OrderBy(gm => gm.OrderNumber) // Maintain module order
            .Select(groupMember => 
            {
                var module = groupMember.AssessmentModule;
                
                // Find progress records for THIS specific module across all students
                var moduleProgresses = assignment.StudentAssignments
                    .SelectMany(eta => eta.ModuleProgress)
                    .Where(mp => mp.AssessmentModuleVersion.AssessmentModuleId == module.Id)
                    .ToList();
                    
                var completedModules = moduleProgresses.Where(isModuleCompleted).ToList();
                
                // Get the latest version for display info
                var latestVersion = module.Versions
                    .OrderByDescending(v => v.Version)
                    .FirstOrDefault();
                
                var moduleCompletionTimes = completedModules
                    .Where(mp => mp is { CompletedAtUtc: not null, StartedAtUtc: not null })
                    .Select(mp => (mp.CompletedAtUtc!.Value - mp.StartedAtUtc!.Value).TotalMinutes)
                    .ToList();
                
                return new ModuleSummaryReportDto
                {
                    ModuleId = module.Id,
                    ModuleTitle = latestVersion?.Title ?? string.Empty,
                    ModuleDescription = latestVersion?.Description,
                    
                    TotalQuestions = latestVersion?.Questions.Count ?? 0,
                    
                    // Handle case where no one started this module
                    CompletionRate = assignment.StudentAssignments.Any() 
                        ? (double)completedModules.Count / assignment.StudentAssignments.Count * 100 
                        : 0,
                    
                    // Scores - could be null if no one completed
                    AverageScore = completedModules.Any() 
                        ? completedModules.Average(m => (double?)m.ScorePercentage) 
                        : null,
                    HighestScore = completedModules.Any() 
                        ? completedModules.Max(m => (double?)m.ScorePercentage) 
                        : null,
                    LowestScore = completedModules.Any() 
                        ? completedModules.Min(m => (double?)m.ScorePercentage) 
                        : null,
                    
                    // Average completion time
                    AverageCompletionTimeMinutes = moduleCompletionTimes.Any() 
                        ? moduleCompletionTimes.Average() 
                        : null
                };
            })
            .ToList();

        // Calculate average completion time for completed assignments
        // Here we don't include assignments with any incomplete modules
        var averageCompletionTimeInMinutes = completedAssignments.Any()
            ? completedAssignments
                .Where(eta => eta.ModuleProgress.All(mp => mp is { CompletedAtUtc: not null, StartedAtUtc: not null }))
                .Select(eta => eta.ModuleProgress
                    .Sum(mp => (mp.CompletedAtUtc!.Value - mp.StartedAtUtc!.Value).TotalMinutes))
                .DefaultIfEmpty(0)
                .Average()
            : 0;
        
        // Build the final assignment report
        var report = new AssignmentSummaryReportDto
        {
            AssignmentId = assignment.Id,
            AssignmentTitle = assignment.Title,
            AssignmentDescription = assignment.Description ?? string.Empty,
            TotalStudents = assignment.StudentAssignments.Count,
            AverageScore = averageScore,

            // Fixed missing properties:
            CompletedStudents = completedAssignments.Count,
            CompletionRate = assignment.StudentAssignments.Any()
                ? (double)completedAssignments.Count / assignment.StudentAssignments.Count * 100
                : 0,
            InProgressStudents = inProgressAssignments.Count,
            NotStartedStudents = notStartedAssignments.Count,

            StartDateUtc = assignment.StartDateUtc,
            EndDateUtc = assignment.EndDateUtc,
            IsActive = assignment.EndDateUtc > DateTime.UtcNow,
            AverageCompletionTimeMinutes = averageCompletionTimeInMinutes,

            // Add the module reports
            ModuleReports = moduleReports
        };
        
        return Response<AssignmentSummaryReportDto, GenericOperationStatuses>.Success(
            report, 
            GenericOperationStatuses.Completed,
            $"Report generated successfully for AssignmentId: {assignmentId}");
    }

    /// <inheritdoc cref="IReportingService.GetStudentReportAsync"/>
    public async Task<Response<StudentReportDto, GenericOperationStatuses>> GetStudentReportAsync(
        string studentId, 
        Guid? assignmentId = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("GetStudentReportAsync request received.");
        Guard.AgainstNullOrWhiteSpace(studentId, nameof(studentId));
        
        var response = await GetStudentReportsAsync(
            new List<string> { studentId }, 
            assignmentId, 
            cancellationToken);

        if (response.IsFailed)
        {
            return Response<StudentReportDto, GenericOperationStatuses>.Failure(
                response.Status, 
                response.Message, 
                response.Errors);
        }
        
        return Response<StudentReportDto, GenericOperationStatuses>.Success(
            response.Data!.First(), 
            response.Status, 
            response.Message);
    }

    /// <inheritdoc cref="IReportingService.GetAllStudentsAsync"/>
    public async Task<Response<PaginatedResponse<IndividualStudentReportDto>, GenericOperationStatuses>> GetAllStudentsAsync(
        string? idFilter,
        string? nameFilter,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("GetAllStudentAssignmentsAsync request received.");
        if (pageNumber < 1)
        {
            logger.LogInformation("Page number {PageNumber} is less than 1. Defaulting to 1", pageNumber);
            pageNumber = 1;
        }
        if (pageSize > options.CurrentValue.MaxPageSize)
        {
            logger.LogInformation("Page size {PageSize} exceeds max of {MaxPageSize}. Capping to max.",
                pageSize,
                options.CurrentValue.MaxPageSize);
            pageSize = options.CurrentValue.MaxPageSize;
        }
        
        // Get distinct student assignments
        var uniqueIds = dbContext.StudentAssignments
            .GroupBy(eta => eta.StudentId)
            .Select(g => g.First().Id); // Any record

        var query = dbContext.StudentAssignments
            .AsNoTracking()
            .Where(eta => uniqueIds.Contains(eta.Id))
            .AsQueryable();

        var totalCount = await query.LongCountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(idFilter))
        {
            query = query.Where(eta => EF.Functions.Like(
                eta.StudentId.ToUpper(), 
                $"%{idFilter.ToUpper()}%"));
        }
        
        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(eta => EF.Functions.Like(
                eta.StudentDisplayName.ToUpper(), 
                $"%{nameFilter.ToUpper()}%"));
        }
        
        var assignments = await query
            .OrderBy(eta => eta.StudentDisplayName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var paginatedResponse = new PaginatedResponse<IndividualStudentReportDto>
        {
            Data = assignments.Select(eta => eta.ConvertToDto()).ToList(),
            TotalCount = totalCount,
            PageSize = assignments.Count
        };
        
        logger.LogDebug("Assembled {Count} student assignments", assignments.Count);
        
        return Response<PaginatedResponse<IndividualStudentReportDto>, GenericOperationStatuses>.Success(
            paginatedResponse, 
            GenericOperationStatuses.Completed,
            "Student assignments retrieved successfully.");
    }

    /// <inheritdoc cref="IReportingService.GetStudentReportsAsync"/>
    public async Task<Response<IList<StudentReportDto>, GenericOperationStatuses>> GetStudentReportsAsync(
        IList<string> studentIds,
        Guid? assignmentId = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("GetStudentReports call received.");
        Guard.AgainstNull(studentIds, nameof(studentIds));
        logger.LogDebug("GetStudentReportsAsync request received for {Count} StudentIds: {StudentIds}", 
            studentIds.Count, string.Join(", ", studentIds));
        
        // Get student assignments
        var studentAssignments = await dbContext.StudentAssignments
            .AsNoTracking()
            .Where(eta => studentIds.Contains(eta.StudentId) && 
                          (!assignmentId.HasValue || eta.AssignmentId == assignmentId.Value))
            .Include(eta => eta.Assignment)
            .ToListAsync(cancellationToken);
        
        // Get existing student progresses
        var studentProgresses = await dbContext.ModuleProgress
            .Where(mp => studentIds.Contains(mp.StudentId) && 
                         (!assignmentId.HasValue || mp.StudentAssignment.AssignmentId == assignmentId.Value))
            .Include(mp => mp.QuestionResponses)
            .Include(mp => mp.StudentAssignment)
                .ThenInclude(eta => eta.Assignment)
            .Include(moduleProgressEntity => moduleProgressEntity.AssessmentModuleVersion)
                .ThenInclude(assessmentModuleVersionEntity => assessmentModuleVersionEntity.Questions)
            .ToListAsync(cancellationToken);
        
        // Get all assignment modules that this student should have
        var expectedModules = await dbContext.Groups
            .AsNoTracking()
            .Where(g => studentAssignments.Select(eta => eta.Assignment.GroupId).Contains(g.Id))
            .SelectMany(g => g.GroupMemberEntities!)
            .Include(gm => gm.AssessmentModule)
                .ThenInclude(am => am.Versions)
            .ToListAsync(cancellationToken);

        // Create missing student progresses for modules without progress
        var existingProgressModuleKeys = studentProgresses
            .Select(up => up.AssessmentModuleVersionId)
            .ToHashSet();

        var missingProgresses = expectedModules
            .SelectMany(em => 
            {
                // If user already has progress for this module, skip
                if (studentProgresses.Any(up => 
                        up.AssessmentModuleVersion.AssessmentModuleId == em.AssessmentModuleId))
                {
                    return [];
                }
                
                var latestVersion = em.AssessmentModule.Versions
                    .OrderByDescending(v => v.Version)
                    .FirstOrDefault();

                if (latestVersion == null || existingProgressModuleKeys.Contains(latestVersion.Id))
                {
                    return [];
                }
                    
                return studentAssignments
                    .Where(eta => eta.Assignment.GroupId == em.GroupId)
                    .Select(eta => new ModuleProgressEntity
                    {
                        StudentId = eta.StudentId,
                        AssessmentModuleVersionId = latestVersion.Id,
                        StartedAtUtc = null,
                        CompletedAtUtc = null,
                        DurationInMinutes = 0,
                        AssessmentModuleVersion = latestVersion,
                        StudentAssignment = eta,
                        QuestionResponses = new List<QuestionResponseEntity>()
                    });
            })
            .ToList();

        // Combine existing and missing progresses
        var completeStudentProgresses = studentProgresses.Concat(missingProgresses).ToList();

        logger.LogDebug("Found {ExistingCount} existing progresses and created {MissingCount} missing progresses for StudentIds: {StudentIds}", 
            studentProgresses.Count, missingProgresses.Count, string.Join(", ", studentIds));
        
        // Create reports for each student
        var reports = new List<StudentReportDto>();
        
        foreach (var studentId in studentIds!)
        {
            // Filter data for this specific student
            var studentSpecificAssignments = studentAssignments.Where(eta => eta.StudentId == studentId).ToList();
            var studentSpecificProgresses = completeStudentProgresses.Where(up => up.StudentId == studentId).ToList();
            
            // Generate module and assignment reports for this student
            var moduleReports = GetModuleReports(studentSpecificProgresses);
            var assignmentReports = GetAssignmentReports(moduleReports);

            var totalAssignments = studentSpecificAssignments.Count;
            var inProgressAssignments = assignmentReports.Count(ar => ar.CompletedModules > 0 && ar.CompletedModules < ar.TotalModules);
            var completedAssignments = assignmentReports.Count(ar => ar.CompletedModules == ar.TotalModules && ar.TotalModules > 0);

            var studentDisplayName = studentSpecificAssignments
                .Select(e => e.StudentDisplayName)
                .FirstOrDefault();
            
            var totalTabSwitches = studentSpecificAssignments.Sum(eta => eta.TabSwitchCount);
            var lastTabSwitch = studentSpecificAssignments
                .Where(eta => eta.LastTabSwitchAtUtc.HasValue)
                .OrderByDescending(eta => eta.LastTabSwitchAtUtc)
                .Select(eta => eta.LastTabSwitchAtUtc)
                .FirstOrDefault();

            var report = new StudentReportDto
            {
                StudentId = studentId,
                DisplayName = string.IsNullOrWhiteSpace(studentDisplayName) ? 
                    $"ID '{studentId}'" : 
                    studentDisplayName,
                TotalAssignments = totalAssignments,
                CompletedAssignments = completedAssignments,
                InProgressAssignments = inProgressAssignments,
                NotStartedAssignments = totalAssignments - (inProgressAssignments + completedAssignments),
                OverallAverageScore = assignmentReports.Any() 
                    ? assignmentReports.Where(ar => ar.ModuleReports.Any(mr => mr.Score.HasValue))
                        .SelectMany(ar => ar.ModuleReports.Where(mr => mr.Score.HasValue))
                        .Average(mr => mr.Score!.Value)
                    : null,
                TotalTimeSpentMinutes = assignmentReports.Sum(ar => ar.TimeSpentMinutes),
                AssignmentProgress = assignmentReports,
                TotalTabSwitchCount = totalTabSwitches,
                LastTabSwitchAtUtc = lastTabSwitch
            };
           
            reports.Add(report);
        }
        
        logger.LogDebug("Assembling report data completed for {Count} students", reports.Count);
        
        return Response<IList<StudentReportDto>, GenericOperationStatuses>.Success(
            reports, 
            GenericOperationStatuses.Completed,
            $"Reports generated successfully for {reports.Count} students");
    }

    /// <summary>
    /// Generates assignment reports from module reports
    /// </summary>
    /// <param name="moduleReports">module reports</param>
    /// <returns>Returns array of <see cref="StudentAssignmentReportDto"/></returns>
    private static List<StudentAssignmentReportDto> GetAssignmentReports(
        Dictionary<AssignmentEntity, HashSet<StudentModuleReportDto>> moduleReports)
    {
        var assignmentReports = new List<StudentAssignmentReportDto>();
        foreach (var kvp in moduleReports)
        {
            var assignmentReport = new StudentAssignmentReportDto
            {
                AssignmentId = kvp.Key.Id,
                AssignmentStartDateUtc = kvp.Key.StartDateUtc,
                AssignmentEndDateUtc = kvp.Key.EndDateUtc,
                AssignmentTitle = kvp.Key.Title,
                StartedAtUtc = kvp.Value.Min(mr => mr.StartedAtUtc),
                CompletedAtUtc = kvp.Value.Max(mr => mr.CompletedAtUtc),
                TimeSpentMinutes = kvp.Value.Sum(mr => mr.TimeSpentMinutes),
                CompletedModules = kvp.Value.Count(mr => mr.Status == ModuleStatus.Completed),
                TotalModules = kvp.Value.Count,
                ModuleReports = kvp.Value,
                TabSwitchCount = kvp.Key.StudentAssignments
                    .FirstOrDefault(eta => eta.AssignmentId == kvp.Key.Id)?.TabSwitchCount ?? 0,
                LastTabSwitchAtUtc = kvp.Key.StudentAssignments
                    .FirstOrDefault(eta => eta.AssignmentId == kvp.Key.Id)?.LastTabSwitchAtUtc
            };
           assignmentReports.Add(assignmentReport);
        }

        return assignmentReports;
    }

    /// <summary>
    /// Gets module reports from student progresses
    /// </summary>
    /// <param name="studentProgresses">Student module progress</param>
    /// <returns>Returns a dictionary where <see cref="AssignmentEntity"/> is a key and
    /// <see cref="StudentModuleReportDto"/> is a value</returns>
    private static Dictionary<AssignmentEntity, HashSet<StudentModuleReportDto>> GetModuleReports(
        List<ModuleProgressEntity> studentProgresses)
    {
        var moduleReports = new Dictionary<AssignmentEntity, HashSet<StudentModuleReportDto>>();
        foreach (var progress in studentProgresses)
        {
            var moduleStatus = DetermineModuleStatus(progress);
            
            var moduleReport = new StudentModuleReportDto
            {
                ModuleId = progress.AssessmentModuleVersion.Id,
                ModuleTitle = progress.AssessmentModuleVersion.Title,
                Status = moduleStatus,
                PassingScore = progress.PassingScorePercentage,
                Score = progress.ScorePercentage,
                Passed = progress.Passed,
                StartedAtUtc = progress.StartedAtUtc,
                CompletedAtUtc = progress.CompletedAtUtc ?? progress.StartedAtUtc?.AddMinutes(progress.DurationInMinutes),
                TimeSpentMinutes = CalculateTimeSpentMinutes(progress),
                AnsweredQuestions = progress.QuestionResponses.Count,
                TotalQuestions = progress.AssessmentModuleVersion.Questions.Count
            };
            
            if (moduleReports.ContainsKey(progress.StudentAssignment.Assignment))
            {
                moduleReports[progress.StudentAssignment.Assignment].Add(moduleReport);
            }
            else
            {
                moduleReports[progress.StudentAssignment.Assignment] = [moduleReport];
            }
        }

        return moduleReports;
    }

    /// <summary>
    /// Calculates the module status based on progress details
    /// </summary>
    /// <param name="progress">Module progress entity</param>
    /// <returns>Returns calculated <see cref="ModuleStatus"/></returns>
    private static ModuleStatus DetermineModuleStatus(ModuleProgressEntity progress)
    {
        // If never started, it's not started
        if (progress.StartedAtUtc == null)
            return ModuleStatus.NotStarted;
            
        if (progress.CompletedAtUtc != null) 
            return ModuleStatus.Completed;
        
        var expirationTime = progress.StartedAtUtc.Value.AddMinutes(progress.DurationInMinutes);
        return expirationTime > DateTime.UtcNow ? ModuleStatus.InProgress : ModuleStatus.Completed;
    }

    /// <summary>
    /// Calculates time spent on a module in minutes
    /// </summary>
    /// <param name="progress">Module progress entity</param>
    /// <returns>Returns time in minute user spent in the module</returns>
    private static int CalculateTimeSpentMinutes(ModuleProgressEntity progress)
    {
        // If never started, no time spent
        if (progress.StartedAtUtc == null)
            return 0;
            
        if (progress.CompletedAtUtc != null)
        {
            // For completed modules, use actual completion time
            return (int)(progress.CompletedAtUtc.Value - progress.StartedAtUtc.Value).TotalMinutes;
        }
        
        var expirationTime = progress.StartedAtUtc.Value.AddMinutes(progress.DurationInMinutes);
        var endTime = DateTime.UtcNow < expirationTime ? DateTime.UtcNow : expirationTime;
        
        // For incomplete modules, use time until now or expiration time, whichever is earlier
        return (int)(endTime - progress.StartedAtUtc.Value).TotalMinutes;
    }
}
