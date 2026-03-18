using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Domain.Enums;
using PublicQ.Infrastructure.Persistence;

namespace PublicQ.Infrastructure.Services;

/// <summary>
/// Implementation of result management logic.
/// </summary>
public class ResultService(
    ApplicationDbContext dbContext,
    IMessageService messageService,
    ILogger<ResultService> logger) : IResultService
{
    public async Task<Response<GenericOperationStatuses>> CalculateClassResultsAsync(
        Guid sessionId, 
        Guid termId, 
        Guid classLevelId, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Calculating results for Session: {Session}, Term: {Term}, Class: {Class}", 
            sessionId, termId, classLevelId);

        // Fetch all assessments for this class in this term/session
        var assessments = await dbContext.StudentAssessments
            .Include(a => a.SubjectScores)
            .Where(a => a.SessionId == sessionId 
                     && a.TermId == termId 
                     && a.ClassLevelId == classLevelId)
            .ToListAsync(cancellationToken);

        if (!assessments.Any())
        {
            return Response<GenericOperationStatuses>.Success(
                GenericOperationStatuses.Completed, 
                "No assessments found to calculate.");
        }

        int numberInClass = assessments.Count;

        // Step 1: Calculate individual totals and averages
        foreach (var assessment in assessments)
        {
            decimal totalScoreObtained = 0;
            decimal totalScoreObtainable = 0;

            foreach (var subject in assessment.SubjectScores)
            {
                // Calculate Test + Exam (Max 100 per subject)
                var test = subject.TestScore ?? 0;
                var exam = subject.ExamScore ?? 0;
                subject.TotalScore = Math.Min(test + exam, 100); 
                subject.Grade = CalculateGrade(subject.TotalScore.Value);
                
                totalScoreObtained += subject.TotalScore.Value;
                totalScoreObtainable += 100; // Assuming each subject is out of 100
            }

            assessment.TotalMarksObtained = totalScoreObtained;
            assessment.TotalMarksObtainable = totalScoreObtainable;
            assessment.NumberInClass = numberInClass;
            
            if (assessment.SubjectScores.Any())
            {
                assessment.AverageScore = totalScoreObtained / assessment.SubjectScores.Count;
                assessment.OverallGrade = CalculateGrade(assessment.AverageScore.Value);
            }
        }

        // Step 2: Rank students by Average Score (Highest to lowest)
        // Groups students by score to handle ties correctly
        var rankedGroups = assessments
            .Where(a => a.AverageScore.HasValue)
            .GroupBy(a => a.AverageScore)
            .OrderByDescending(g => g.Key)
            .ToList();

        int currentRank = 1;
        foreach (var group in rankedGroups)
        {
            foreach (var student in group)
            {
                student.PositionInClass = currentRank;
            }
            // If two people tie for 1st, the next person is 3rd.
            currentRank += group.Count(); 
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Calculated and ranked {Count} students successfully.", assessments.Count);
        return Response<GenericOperationStatuses>.Success(
            GenericOperationStatuses.Completed, 
            "Class results calculated and ranked successfully.");
    }

    public async Task<Response<GenericOperationStatuses>> UpdateAssessmentStatusAsync(
        Guid assessmentId, 
        ModerationStatus newStatus, 
        CancellationToken cancellationToken = default)
    {
        var assessment = await dbContext.StudentAssessments
            .FirstOrDefaultAsync(a => a.Id == assessmentId, cancellationToken);

        if (assessment == null)
        {
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assessment not found.");
        }

        assessment.Status = newStatus;
        bool isNewlyPublished = false;

        if (newStatus == ModerationStatus.Published && assessment.PublishedAt == null)
        {
            assessment.PublishedAt = DateTime.UtcNow;
            isNewlyPublished = true;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        if (isNewlyPublished)
        {
            await NotifyParentsAsync(new[] { assessment.ExamTakerId }, cancellationToken);
        }

        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Status updated.");
    }

    public async Task<Response<GenericOperationStatuses>> UpdateAssessmentDetailsAsync(
        Guid assessmentId, 
        UpdateAssessmentDetailsDto details, 
        CancellationToken cancellationToken = default)
    {
        var assessment = await dbContext.StudentAssessments
            .FirstOrDefaultAsync(a => a.Id == assessmentId, cancellationToken);

        if (assessment == null)
        {
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assessment not found.");
        }

        // Update Attendance
        assessment.TimesSchoolOpened = details.TimesSchoolOpened ?? assessment.TimesSchoolOpened;
        assessment.TimesPresent = details.TimesPresent ?? assessment.TimesPresent;
        assessment.TimesAbsent = details.TimesAbsent ?? assessment.TimesAbsent;

        // Update Affective Domain
        assessment.Regularity = details.Regularity ?? assessment.Regularity;
        assessment.Punctuality = details.Punctuality ?? assessment.Punctuality;
        assessment.Neatness = details.Neatness ?? assessment.Neatness;
        assessment.AttitudeInSchool = details.AttitudeInSchool ?? assessment.AttitudeInSchool;
        assessment.SocialActivities = details.SocialActivities ?? assessment.SocialActivities;

        // Update Psychomotor
        assessment.IndoorGames = details.IndoorGames ?? assessment.IndoorGames;
        assessment.FieldGames = details.FieldGames ?? assessment.FieldGames;
        assessment.TrackGames = details.TrackGames ?? assessment.TrackGames;
        assessment.Jumps = details.Jumps ?? assessment.Jumps;
        assessment.Swims = details.Swims ?? assessment.Swims;

        // Update Remarks
        assessment.ClassTeacherComment = details.ClassTeacherComment ?? assessment.ClassTeacherComment;
        assessment.HeadTeacherComment = details.HeadTeacherComment ?? assessment.HeadTeacherComment;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Assessment details updated.");
    }

    public async Task<Response<GenericOperationStatuses>> ToggleAssessmentLockAsync(
        Guid assessmentId, 
        bool isLocked, 
        CancellationToken cancellationToken = default)
    {
        var assessment = await dbContext.StudentAssessments
            .FirstOrDefaultAsync(a => a.Id == assessmentId, cancellationToken);

        if (assessment == null)
        {
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assessment not found.");
        }

        assessment.IsLockedForParents = isLocked;
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Response<GenericOperationStatuses>.Success(
            GenericOperationStatuses.Completed, 
            $"Assessment {(isLocked ? "locked" : "unlocked")} successfully.");
    }

    public async Task<Response<GenericOperationStatuses>> BatchUpdateClassStatusAsync(
        Guid sessionId, 
        Guid termId, 
        Guid classLevelId, 
        ModerationStatus currentStatus,
        ModerationStatus newStatus, 
        CancellationToken cancellationToken = default)
    {
        var assessmentsToUpdate = await dbContext.StudentAssessments
            .Where(a => a.SessionId == sessionId 
                     && a.TermId == termId 
                     && a.ClassLevelId == classLevelId
                     && a.Status == currentStatus)
            .ToListAsync(cancellationToken);

        if (!assessmentsToUpdate.Any())
        {
            return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "No matching assessments to update.");
        }

        var newlyPublishedExamTakerIds = new List<string>();

        foreach (var a in assessmentsToUpdate)
        {
            a.Status = newStatus;
            if (newStatus == ModerationStatus.Published && a.PublishedAt == null)
            {
                a.PublishedAt = DateTime.UtcNow;
                newlyPublishedExamTakerIds.Add(a.ExamTakerId);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        if (newlyPublishedExamTakerIds.Any())
        {
            await NotifyParentsAsync(newlyPublishedExamTakerIds, cancellationToken);
        }

        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, $"Batch updated {assessmentsToUpdate.Count} records to {newStatus}.");
    }

    public async Task<Response<IList<AssessmentReportDto>, GenericOperationStatuses>> GetClassAssessmentsAsync(
        Guid sessionId, 
        Guid termId, 
        Guid classLevelId, 
        CancellationToken cancellationToken = default)
    {
        var assessments = await dbContext.StudentAssessments
            .Include(a => a.ExamTaker)
            .Where(a => a.SessionId == sessionId 
                     && a.TermId == termId 
                     && a.ClassLevelId == classLevelId)
            .Select(a => new AssessmentReportDto
            {
                Id = a.Id,
                ExamTakerId = a.ExamTakerId,
                StudentName = a.ExamTaker.FullName ?? a.ExamTaker.Email ?? "Unknown",
                AdmissionNumber = a.ExamTaker.AdmissionNumber,
                Status = a.Status,
                IsLockedForParents = a.IsLockedForParents,
                TotalMarksObtained = a.TotalMarksObtained,
                AverageScore = a.AverageScore,
                PositionInClass = a.PositionInClass,
                OverallGrade = a.OverallGrade,
                CreatedAt = a.CreatedAt,
                PublishedAt = a.PublishedAt
            })
            .ToListAsync(cancellationToken);

        return Response<IList<AssessmentReportDto>, GenericOperationStatuses>.Success(
            assessments, 
            GenericOperationStatuses.Completed, 
            "Fetched class assessments successfully.");
    }

    public async Task<Response<AssessmentDetailsDto, GenericOperationStatuses>> GetAssessmentDetailsAsync(
        Guid assessmentId, 
        CancellationToken cancellationToken = default)
    {
        var assessment = await dbContext.StudentAssessments
            .Include(a => a.ExamTaker)
            .Include(a => a.SubjectScores)
                .ThenInclude(s => s.Subject)
            .FirstOrDefaultAsync(a => a.Id == assessmentId, cancellationToken);

        if (assessment == null)
        {
            return Response<AssessmentDetailsDto, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound, "Assessment not found.");
        }

        var dto = new AssessmentDetailsDto
        {
            Id = assessment.Id,
            ExamTakerId = assessment.ExamTakerId,
            StudentName = assessment.ExamTaker.FullName ?? assessment.ExamTaker.Email ?? "Unknown",
            AdmissionNumber = assessment.ExamTaker.AdmissionNumber,
            Status = assessment.Status,
            TotalMarksObtained = assessment.TotalMarksObtained,
            TotalMarksObtainable = assessment.TotalMarksObtainable,
            AverageScore = assessment.AverageScore,
            PositionInClass = assessment.PositionInClass,
            NumberInClass = assessment.NumberInClass,
            OverallGrade = assessment.OverallGrade,
            
            TimesSchoolOpened = assessment.TimesSchoolOpened,
            TimesPresent = assessment.TimesPresent,
            TimesAbsent = assessment.TimesAbsent,
            
            Regularity = assessment.Regularity,
            Punctuality = assessment.Punctuality,
            Neatness = assessment.Neatness,
            AttitudeInSchool = assessment.AttitudeInSchool,
            SocialActivities = assessment.SocialActivities,
            
            IndoorGames = assessment.IndoorGames,
            FieldGames = assessment.FieldGames,
            TrackGames = assessment.TrackGames,
            Jumps = assessment.Jumps,
            Swims = assessment.Swims,
            
            ClassTeacherComment = assessment.ClassTeacherComment,
            HeadTeacherComment = assessment.HeadTeacherComment,
            
            SubjectScores = assessment.SubjectScores.Select(s => new StudentSubjectScoreDto
            {
                ExamTakerId = assessment.ExamTakerId,
                TestScore = s.TestScore,
                ExamScore = s.ExamScore,
                SubjectRemark = s.SubjectRemark
                // Note: The SubjectName is not currently in StudentSubjectScoreDto, but could be added if needed, or fetched alongside.
            }).ToList()
        };

        return Response<AssessmentDetailsDto, GenericOperationStatuses>.Success(
            dto, GenericOperationStatuses.Completed, "Fetched assessment details successfully.");
    }

    public async Task<Response<GenericOperationStatuses>> SaveBulkScoresAsync(
        BulkScoreEntryDto dto, 
        CancellationToken cancellationToken = default)
    {
        // 1. Fetch existing assessments for this context to link scores
        var existingAssessments = await dbContext.StudentAssessments
            .Include(a => a.SubjectScores)
            .Where(a => a.SessionId == dto.SessionId 
                     && a.TermId == dto.TermId 
                     && a.ClassLevelId == dto.ClassLevelId)
            .ToListAsync(cancellationToken);

        foreach (var scoreEntry in dto.Scores)
        {
            // Find or create the master assessment record for this student
            var assessment = existingAssessments.FirstOrDefault(a => a.ExamTakerId == scoreEntry.ExamTakerId);
            
            if (assessment == null)
            {
                assessment = new Persistence.Entities.Academic.StudentAssessmentEntity
                {
                    Id = Guid.NewGuid(),
                    ExamTakerId = scoreEntry.ExamTakerId,
                    SessionId = dto.SessionId,
                    TermId = dto.TermId,
                    ClassLevelId = dto.ClassLevelId,
                    Status = ModerationStatus.Draft, // Starting status
                    CreatedAt = DateTime.UtcNow
                };
                dbContext.StudentAssessments.Add(assessment);
                existingAssessments.Add(assessment); // Add to local list in case of duplicate entries
            }

            // Find or create the specific subject score record inside the assessment
            var subjectScore = assessment.SubjectScores.FirstOrDefault(s => s.SubjectId == dto.SubjectId);
            if (subjectScore == null)
            {
                subjectScore = new Persistence.Entities.Academic.SubjectScoreEntity
                {
                    Id = Guid.NewGuid(),
                    StudentAssessmentId = assessment.Id,
                    SubjectId = dto.SubjectId
                };
                assessment.SubjectScores.Add(subjectScore);
            }

            // Update scores
            subjectScore.TestScore = scoreEntry.TestScore;
            subjectScore.ExamScore = scoreEntry.ExamScore;
            subjectScore.SubjectRemark = scoreEntry.SubjectRemark;

            // Recalculate this specific subject total to keep the UI in sync before final Calc results
            var test = subjectScore.TestScore ?? 0;
            var exam = subjectScore.ExamScore ?? 0;
            subjectScore.TotalScore = Math.Min(test + exam, 100);
            subjectScore.Grade = CalculateGrade(subjectScore.TotalScore.Value);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Response<GenericOperationStatuses>.Success(
            GenericOperationStatuses.Completed, 
            "Bulk scores saved successfully.");
    }

    /// <summary>
    /// Basic hardcoded grading scale.
    /// </summary>
    public string CalculateGrade(decimal score)
    {
        return score switch
        {
            >= 75 => "A1",
            >= 70 => "B2",
            >= 65 => "B3",
            >= 60 => "C4",
            >= 55 => "C5",
            >= 50 => "C6",
            >= 45 => "D7",
            >= 40 => "E8",
            _ => "F9"
        };
    }

    private async Task NotifyParentsAsync(IEnumerable<string> examTakerIds, CancellationToken cancellationToken)
    {
        try
        {
            var parentEmails = await dbContext.ParentStudentLinks
                .Include(l => l.Parent)
                .Where(l => examTakerIds.Contains(l.StudentId) && !string.IsNullOrEmpty(l.Parent.Email))
                .Select(l => l.Parent.Email)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!parentEmails.Any()) return;

            var req = new SendMessageRequest
            {
                Subject = "Student Result Published - ExamNova",
                Body = "Dear Parent/Guardian, \n\nThe academic result for your ward has been processed and officially published. You can now log into the ExamNova Parent Portal to view the comprehensive Report Card.\n\nThank you,\nExamNova Administration",
                Recipients = parentEmails
            };

            await messageService.SendAsync(req, cancellationToken);
            logger.LogInformation("Sent publication notification to {Count} parents.", parentEmails.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send parent result notifications.");
        }
    }
}
