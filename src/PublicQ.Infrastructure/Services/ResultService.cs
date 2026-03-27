using Microsoft.EntityFrameworkCore;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Application.Models.Academic;
using PublicQ.Domain.Enums;
using PublicQ.Infrastructure.Persistence;
using PublicQ.Infrastructure.Persistence.Entities.Academic;
using PublicQ.Shared;
using System.Globalization;

namespace PublicQ.Infrastructure.Services;

public class ResultService(ApplicationDbContext dbContext) : IResultService
{
    public async Task<Response<ResultUploadResponse, GenericOperationStatuses>> UploadResultCsvAsync(Stream fileStream, Guid sessionId, Guid termId, Guid classLevelId, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        using var reader = new StreamReader(fileStream);
        var lines = new List<string>();
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            lines.Add(line);
        }

        if (lines.Count < 30)
        {
            return Response<ResultUploadResponse, GenericOperationStatuses>.Failure(GenericOperationStatuses.BadRequest, "Invalid CSV format. Expected Mercy's Gate Report Card format.");
        }

        try
        {
            // Row 8: Student’s/Pupil’s Name,STUDENT 1,,,,,,Class,SSS1
            var row8 = lines[7].Split(',');
            string studentName = row8[1].Trim();
            
            // Row 33-35: Attendance
            var row33 = lines[32].Split(','); 
            var row34 = lines[33].Split(','); 
            var row35 = lines[34].Split(','); 

            int schoolDays = int.TryParse(row33[1], out var sd) ? sd : 0;
            int daysAttended = int.TryParse(row34[1], out var da) ? da : 0;
            int daysAbsent = int.TryParse(row35[1], out var dab) ? dab : 0;

            // Row 44: Teacher’s Comments
            var row44 = lines[43].Split(',');
            string teacherComment = row44[3].Trim();

            // Row 46: Principal’s Comments
            var row46 = lines[45].Split(',');
            string headTeacherComment = row46[3].Trim();

            // Find the student
            var student = await dbContext.Students
                .FirstOrDefaultAsync(s => s.FullName == studentName || s.AdmissionNumber == studentName, cancellationToken);

            if (student == null)
            {
                return Response<ResultUploadResponse, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, $"Student '{studentName}' not found.");
            }

            // Create or update assessment
            var assessment = await dbContext.StudentAssessments
                .FirstOrDefaultAsync(a => a.StudentId == student.Id && a.SessionId == sessionId && a.TermId == termId, cancellationToken);

            if (assessment == null)
            {
                assessment = new StudentAssessmentEntity
                {
                    Id = Guid.NewGuid(),
                    StudentId = student.Id,
                    SessionId = sessionId,
                    TermId = termId,
                    ClassLevelId = classLevelId
                };
                dbContext.StudentAssessments.Add(assessment);
            }

            assessment.TimesSchoolOpened = schoolDays;
            assessment.TimesPresent = daysAttended;
            assessment.TimesAbsent = daysAbsent;
            assessment.ClassTeacherComment = teacherComment;
            assessment.HeadTeacherComment = headTeacherComment;
            assessment.Status = ModerationStatus.Draft;

            // Parse Subjects
            var existingScores = await dbContext.SubjectScores
                .Where(s => s.StudentAssessmentId == assessment.Id)
                .ToListAsync(cancellationToken);
            dbContext.SubjectScores.RemoveRange(existingScores);

            for (int i = 12; i < lines.Count && i < 28; i++)
            {
                var row = lines[i].Split(',');
                if (row.Length < 4 || string.IsNullOrWhiteSpace(row[0])) continue;

                string subjectName = row[0].Trim();
                if (subjectName.Equals("Total", StringComparison.OrdinalIgnoreCase)) break;

                decimal testScore = decimal.TryParse(row[1], out var ts) ? (ts > 40 ? 40 : ts) : 0;
                decimal examScore = decimal.TryParse(row[2], out var es) ? (es > 60 ? 60 : es) : 0;
                
                // Total is always sum of test and exam
                decimal totalScore = testScore + examScore;
                string grade = row[7].Trim();
                string remark = row[8].Trim();

                var subject = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == subjectName, cancellationToken);
                if (subject == null) continue;

                dbContext.SubjectScores.Add(new SubjectScoreEntity
                {
                    Id = Guid.NewGuid(),
                    StudentAssessmentId = assessment.Id,
                    SubjectId = subject.Id,
                    TestScore = testScore,
                    ExamScore = examScore,
                    TotalScore = totalScore,
                    Grade = grade,
                    SubjectRemark = remark
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return Response<ResultUploadResponse, GenericOperationStatuses>.Success(new ResultUploadResponse(1, 1, 0, []), GenericOperationStatuses.Completed);
        }
        catch (Exception ex)
        {
            return Response<ResultUploadResponse, GenericOperationStatuses>.Failure(GenericOperationStatuses.BadRequest, $"Error: {ex.Message}");
        }
    }

    public async Task<Response<StudentAssessmentDto, GenericOperationStatuses>> GetStudentAssessmentAsync(Guid assessmentId, CancellationToken cancellationToken)
    {
        var assessment = await dbContext.StudentAssessments
            .Include(a => a.Student)
            .Include(a => a.Session)
            .Include(a => a.Term)
            .Include(a => a.ClassLevel)
            .Include(a => a.SubjectScores)
                .ThenInclude(s => s.Subject)
            .Where(a => a.Id == assessmentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (assessment == null) return Response<StudentAssessmentDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assessment not found.");
        return Response<StudentAssessmentDto, GenericOperationStatuses>.Success(MapToDto(assessment), GenericOperationStatuses.Completed);
    }

    public async Task<Response<IEnumerable<StudentAssessmentDto>, GenericOperationStatuses>> GetClassAssessmentsAsync(Guid sessionId, Guid termId, Guid classLevelId, CancellationToken cancellationToken)
    {
        var assessments = await dbContext.StudentAssessments
            .Include(a => a.Student)
            .Include(a => a.Session)
            .Include(a => a.Term)
            .Include(a => a.ClassLevel)
            .Include(a => a.SubjectScores)
                .ThenInclude(s => s.Subject)
            .Where(a => a.SessionId == sessionId && a.TermId == termId && a.ClassLevelId == classLevelId)
            .Select(a => MapToDto(a))
            .ToListAsync(cancellationToken);

        return Response<IEnumerable<StudentAssessmentDto>, GenericOperationStatuses>.Success(assessments, GenericOperationStatuses.Completed);
    }

    public async Task<Response<IEnumerable<StudentAssessmentDto>, GenericOperationStatuses>> GetParentChildrenResultsAsync(string parentUserId, CancellationToken cancellationToken)
    {
        var studentIds = await dbContext.ParentStudentLinks
            .Where(l => l.ParentId == parentUserId)
            .Select(l => l.StudentId)
            .ToListAsync(cancellationToken);

        var assessments = await dbContext.StudentAssessments
            .Include(a => a.Student)
            .Include(a => a.Session)
            .Include(a => a.Term)
            .Include(a => a.ClassLevel)
            .Include(a => a.SubjectScores)
                .ThenInclude(s => s.Subject)
            .Where(a => studentIds.Contains(a.StudentId) && a.Status == ModerationStatus.Published)
            .Select(a => MapToDto(a))
            .ToListAsync(cancellationToken);

        return Response<IEnumerable<StudentAssessmentDto>, GenericOperationStatuses>.Success(assessments, GenericOperationStatuses.Completed);
    }

    public async Task<Response<GenericOperationStatuses>> UpdateStatusAsync(Guid assessmentId, ModerationStatus status, CancellationToken cancellationToken)
    {
        var assessment = await dbContext.StudentAssessments.FindAsync([assessmentId], cancellationToken);
        if (assessment == null) return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assessment not found.");
        
        assessment.Status = status;
        if (status == ModerationStatus.Published) assessment.PublishedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed);
    }

    public async Task<Response<AssessmentDetailsDto, GenericOperationStatuses>> GetAssessmentDetailsAsync(Guid assessmentId, CancellationToken cancellationToken)
    {
        var assessment = await dbContext.StudentAssessments
            .Include(a => a.Student)
            .Include(a => a.SubjectScores)
                .ThenInclude(s => s.Subject)
            .FirstOrDefaultAsync(a => a.Id == assessmentId, cancellationToken);

        if (assessment == null) return Response<AssessmentDetailsDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assessment not found.");

        return Response<AssessmentDetailsDto, GenericOperationStatuses>.Success(new AssessmentDetailsDto
        {
            Id = assessment.Id,
            StudentId = assessment.StudentId,
            StudentName = assessment.Student?.FullName ?? "Unknown",
            AdmissionNumber = assessment.Student?.AdmissionNumber,
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
            ClassTeacherComment = assessment.ClassTeacherComment,
            HeadTeacherComment = assessment.HeadTeacherComment,
            SubjectScores = assessment.SubjectScores.Select(s => new StudentSubjectScoreDto
            {
                StudentId = assessment.StudentId,
                SubjectName = s.Subject?.Name,
                TestScore = s.TestScore,
                ExamScore = s.ExamScore,
                TotalScore = s.TotalScore,
                Grade = s.Grade,
                SubjectRemark = s.SubjectRemark
            }).ToList()
        }, GenericOperationStatuses.Completed);
    }

    public async Task<Response<GenericOperationStatuses>> SaveBulkScoresAsync(BulkScoreEntryDto request, CancellationToken cancellationToken)
    {
        var classLevel = await dbContext.ClassLevels
            .Include(c => c.GradingSchema)
                .ThenInclude(s => s != null ? s.GradeRanges : null)
            .FirstOrDefaultAsync(c => c.Id == request.ClassLevelId, cancellationToken);
            
        foreach (var entry in request.Scores)
        {
            var student = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == entry.StudentId || s.AdmissionNumber == entry.StudentId, cancellationToken);
            if (student == null) continue;

            // Determine SubjectId
            var subjectId = entry.SubjectId ?? request.SubjectId;
            if (subjectId == null || subjectId == Guid.Empty) continue;

            var assessment = await dbContext.StudentAssessments
                .FirstOrDefaultAsync(a => a.StudentId == student.Id && a.SessionId == request.SessionId && a.TermId == request.TermId, cancellationToken);

            if (assessment == null)
            {
                assessment = new StudentAssessmentEntity
                {
                    Id = Guid.NewGuid(),
                    StudentId = student.Id,
                    SessionId = request.SessionId,
                    TermId = request.TermId,
                    ClassLevelId = request.ClassLevelId,
                    Status = ModerationStatus.Draft
                };
                dbContext.StudentAssessments.Add(assessment);
            }

            var score = await dbContext.SubjectScores
                .FirstOrDefaultAsync(s => s.StudentAssessmentId == assessment.Id && s.SubjectId == subjectId.Value, cancellationToken);

            if (score == null)
            {
                score = new SubjectScoreEntity
                {
                    Id = Guid.NewGuid(),
                    StudentAssessmentId = assessment.Id,
                    SubjectId = subjectId.Value
                };
                dbContext.SubjectScores.Add(score);
            }

            score.TestScore = (entry.TestScore ?? 0) > 40 ? 40 : (entry.TestScore ?? 0);
            score.ExamScore = (entry.ExamScore ?? 0) > 60 ? 60 : (entry.ExamScore ?? 0);
            score.TotalScore = score.TestScore + score.ExamScore;
            // Re-calculate Grade and Remark immediately
            var scoreVal = (int)Math.Round(score.TotalScore.Value);
            var gradeRange = classLevel?.GradingSchema?.GradeRanges?.FirstOrDefault(r => scoreVal >= r.MinScore && scoreVal <= r.MaxScore);
            score.Grade = gradeRange?.Symbol ?? "-";
            score.SubjectRemark = gradeRange?.Remark ?? "-";
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Scores saved successfully.");
    }

    public async Task<Response<GenericOperationStatuses>> CalculateClassResultsAsync(Guid sessionId, Guid termId, Guid classLevelId, CancellationToken cancellationToken)
    {
        var classLevel = await dbContext.ClassLevels
            .Include(c => c.GradingSchema)
                .ThenInclude(s => s != null ? s.GradeRanges : null)
            .FirstOrDefaultAsync(c => c.Id == classLevelId, cancellationToken);

        var assessments = await dbContext.StudentAssessments
            .Include(a => a.SubjectScores)
            .Where(a => a.SessionId == sessionId && a.TermId == termId && a.ClassLevelId == classLevelId)
            .ToListAsync(cancellationToken);

        if (!assessments.Any()) return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "No assessments found for this class.");

        var schema = classLevel?.GradingSchema;
        var gradeRanges = schema?.GradeRanges.OrderByDescending(r => r.MinScore).ToList();

        foreach (var a in assessments)
        {
            foreach (var s in a.SubjectScores)
            {
                if (gradeRanges != null && s.TotalScore.HasValue)
                {
                    var scoreVal = (int)Math.Round(s.TotalScore.Value);
                    var range = gradeRanges.FirstOrDefault(r => scoreVal >= r.MinScore && scoreVal <= r.MaxScore);
                    if (range != null)
                    {
                        s.Grade = range.Symbol;
                        s.SubjectRemark = range.Remark;
                    }
                }
            }

            a.TotalMarksObtained = a.SubjectScores.Sum(s => s.TotalScore);
            a.TotalMarksObtainable = a.SubjectScores.Count * 100;
            a.AverageScore = a.TotalMarksObtainable > 0 ? (a.TotalMarksObtained / a.TotalMarksObtainable) * 100 : 0;
        }

        var ranked = assessments.OrderByDescending(a => a.AverageScore).ToList();
        for (int i = 0; i < ranked.Count; i++)
        {
            ranked[i].PositionInClass = i + 1;
            ranked[i].NumberInClass = ranked.Count;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed);
    }

    public async Task<Response<GenericOperationStatuses>> UpdateAssessmentStatusAsync(Guid assessmentId, ModerationStatus newStatus, CancellationToken cancellationToken)
    {
        return await UpdateStatusAsync(assessmentId, newStatus, cancellationToken);
    }

    public async Task<Response<GenericOperationStatuses>> UpdateAssessmentDetailsAsync(Guid assessmentId, UpdateAssessmentDetailsDto request, CancellationToken cancellationToken)
    {
        var assessment = await dbContext.StudentAssessments.FindAsync([assessmentId], cancellationToken);
        if (assessment == null) return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assessment not found.");

        assessment.TimesSchoolOpened = request.TimesSchoolOpened ?? assessment.TimesSchoolOpened;
        assessment.TimesPresent = request.TimesPresent ?? assessment.TimesPresent;
        assessment.TimesAbsent = request.TimesAbsent ?? assessment.TimesAbsent;
        assessment.ClassTeacherComment = request.ClassTeacherComment ?? assessment.ClassTeacherComment;
        assessment.HeadTeacherComment = request.HeadTeacherComment ?? assessment.HeadTeacherComment;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed);
    }

    public async Task<Response<GenericOperationStatuses>> ToggleAssessmentLockAsync(Guid assessmentId, bool isLocked, CancellationToken cancellationToken)
    {
        var assessment = await dbContext.StudentAssessments.FindAsync([assessmentId], cancellationToken);
        if (assessment == null) return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Assessment not found.");

        assessment.IsLockedForParents = isLocked;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed);
    }

    public async Task<Response<GenericOperationStatuses>> BatchUpdateClassStatusAsync(Guid sessionId, Guid termId, Guid classLevelId, ModerationStatus currentStatus, ModerationStatus newStatus, CancellationToken cancellationToken)
    {
        var assessments = await dbContext.StudentAssessments
            .Where(a => a.SessionId == sessionId && a.TermId == termId && a.ClassLevelId == classLevelId && a.Status == currentStatus)
            .ToListAsync(cancellationToken);

        foreach (var a in assessments)
        {
            a.Status = newStatus;
            if (newStatus == ModerationStatus.Published) a.PublishedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed);
    }

    public async Task<Response<GenericOperationStatuses>> SyncOnlineScoresAsync(Guid sessionId, Guid termId, Guid classLevelId, CancellationToken cancellationToken)
    {
        try 
        {
            var assessments = await dbContext.StudentAssessments
                .Where(a => a.SessionId == sessionId && a.TermId == termId && a.ClassLevelId == classLevelId)
                .Include(a => a.SubjectScores)
                .ToListAsync(cancellationToken);

            if (!assessments.Any()) return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "No assessments found for this class.");

            var studentIds = assessments.Select(a => a.StudentId).ToList();

            var onlineScores = await dbContext.StudentAssignments
                .Where(eta => studentIds.Contains(eta.StudentId) && eta.Assignment.IsPublished && eta.Assignment.SubjectId != null)
                .Include(eta => eta.Assignment)
                .Include(eta => eta.ModuleProgress)
                    .ThenInclude(mp => mp.QuestionResponses)
                .Include(eta => eta.ModuleProgress)
                    .ThenInclude(mp => mp.AssessmentModuleVersion)
                        .ThenInclude(amv => amv.Questions)
                .ToListAsync(cancellationToken);
        
            foreach (var assessment in assessments)
            {
                var studentScores = onlineScores.Where(s => s != null && s.StudentId == assessment.StudentId).ToList();
                
                foreach (var onlineScore in studentScores)
                {
                    if (onlineScore.Assignment == null || onlineScore.Assignment.SubjectId == null)
                        continue;

                    var subjectId = onlineScore.Assignment.SubjectId.Value;
                    
                    decimal totalPercentage = 0;
                    int completedModules = 0;

                    if (onlineScore.ModuleProgress != null)
                    {
                        foreach (var mp in onlineScore.ModuleProgress.Where(mp => mp != null && mp.CompletedAtUtc != null))
                        {
                            totalPercentage += mp.ScorePercentage ?? 0;
                            completedModules++;
                        }
                    }

                    if (completedModules == 0) continue;

                    decimal averagePercentage = totalPercentage / completedModules;
                    decimal scaledScore = (averagePercentage / 100) * 60;

                    assessment.SubjectScores ??= new List<SubjectScoreEntity>();
                    var scoreEntity = assessment.SubjectScores.FirstOrDefault(s => s != null && s.SubjectId == subjectId);
                    if (scoreEntity == null)
                    {
                        scoreEntity = new SubjectScoreEntity
                        {
                            Id = Guid.NewGuid(),
                            StudentAssessmentId = assessment.Id,
                            SubjectId = subjectId
                        };
                        dbContext.SubjectScores.Add(scoreEntity);
                        assessment.SubjectScores.Add(scoreEntity);
                    }

                    scoreEntity.ExamScore = Math.Round(scaledScore, 2);
                    scoreEntity.TotalScore = (scoreEntity.TestScore ?? 0) + scoreEntity.ExamScore;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Online scores synchronized successfully.");
        }
        catch (Exception ex)
        {
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.BadRequest, $"Sync failed: {ex.Message} {ex.InnerException?.Message} | Trace: {ex.StackTrace}");
        }
    }

    private static StudentAssessmentDto MapToDto(StudentAssessmentEntity a)
    {
        return new StudentAssessmentDto(
            a.Id,
            a.StudentId,
            a.Student?.FullName ?? "Unknown",
            a.Student?.AdmissionNumber ?? "N/A",
            a.Session?.Name ?? "N/A",
            a.Term?.Name ?? "N/A",
            a.ClassLevel?.Name ?? "N/A",
            a.Status,
            a.TotalMarksObtained,
            a.TotalMarksObtainable,
            a.AverageScore,
            a.PositionInClass,
            a.NumberInClass,
            a.OverallGrade,
            a.TimesSchoolOpened,
            a.TimesPresent,
            a.TimesAbsent,
            a.ClassTeacherComment,
            a.HeadTeacherComment,
            a.SubjectScores.Select(s => new SubjectScoreDto(
                s.Id,
                s.Subject?.Name ?? "Unknown",
                s.TestScore,
                s.ExamScore,
                s.TotalScore,
                s.Grade,
                s.SubjectRemark
            )).ToList(),
            a.PublishedAt,
            a.IsLockedForParents
        );
    }
}
