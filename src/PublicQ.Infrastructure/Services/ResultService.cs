using Microsoft.EntityFrameworkCore;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models.Academic;
using PublicQ.Domain.Enums;
using PublicQ.Infrastructure.Persistence;
using PublicQ.Infrastructure.Persistence.Entities.Academic;
using System.Globalization;

namespace PublicQ.Infrastructure.Services;

public class ResultService(ApplicationDbContext dbContext) : IResultService
{
    public async Task<ResultUploadResponse> UploadResultCsvAsync(Stream fileStream, Guid sessionId, Guid termId, Guid classLevelId)
    {
        var errors = new List<string>();
        int successCount = 0;
        int failureCount = 0;

        using var reader = new StreamReader(fileStream);
        var lines = new List<string>();
        while (!reader.EndOfStream)
        {
            lines.Add(await reader.ReadLineAsync() ?? "");
        }

        if (lines.Count < 50)
        {
            return new ResultUploadResponse(0, 0, 1, ["Invalid CSV format. Expected Mercy's Gate Report Card format (approx 52 lines)."]);
        }

        try
        {
            // Parsing Logic based on the provided CSV structure
            // Row 8: Student’s/Pupil’s Name,STUDENT 1,,,,,,Class,SSS1
            var row8 = lines[7].Split(',');
            string studentName = row8[1].Trim();
            
            // Row 33-35: Attendance
            var row33 = lines[32].Split(','); // School Days,120
            var row34 = lines[33].Split(','); // Days Attended,108
            var row35 = lines[34].Split(','); // Days Absent,12

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
            var student = await dbContext.ExamTakers
                .FirstOrDefaultAsync(s => s.FullName == studentName || s.AdmissionNumber == studentName);

            if (student == null)
            {
                return new ResultUploadResponse(0, 0, 1, [$"Student '{studentName}' not found in the system. Please register the student first."]);
            }

            // Create or update assessment
            var assessment = await dbContext.StudentAssessments
                .FirstOrDefaultAsync(a => a.ExamTakerId == student.Id && a.SessionId == sessionId && a.TermId == termId);

            if (assessment == null)
            {
                assessment = new StudentAssessmentEntity
                {
                    Id = Guid.NewGuid(),
                    ExamTakerId = student.Id,
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

            // Parse Subjects (Rows 13 to approx 28)
            // Row 11: SUBJECTS,TEST,EXAM,TOTAL,2ND TERM,3RD TERM,AVRGE,GRADE,RMKS
            
            // Clear existing scores for this assessment to avoid duplicates if re-uploading
            var existingScores = await dbContext.SubjectScores
                .Where(s => s.StudentAssessmentId == assessment.Id)
                .ToListAsync();
            dbContext.SubjectScores.RemoveRange(existingScores);

            for (int i = 12; i < 28; i++)
            {
                var row = lines[i].Split(',');
                if (row.Length < 4 || string.IsNullOrWhiteSpace(row[0])) continue;

                string subjectName = row[0].Trim();
                if (subjectName.Equals("Total", StringComparison.OrdinalIgnoreCase)) break;

                decimal testScore = decimal.TryParse(row[1], out var ts) ? ts : 0;
                decimal examScore = decimal.TryParse(row[2], out var es) ? es : 0;
                decimal totalScore = decimal.TryParse(row[3], out var tot) ? tot : 0;
                string grade = row[7].Trim();
                string remark = row[8].Trim();

                // Find subject
                var subject = await dbContext.Subjects
                    .FirstOrDefaultAsync(s => s.Name == subjectName);

                if (subject == null)
                {
                    errors.Add($"Subject '{subjectName}' not found. Skipping score for this subject.");
                    continue;
                }

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

            await dbContext.SaveChangesAsync();
            successCount = 1;

            return new ResultUploadResponse(1, successCount, failureCount, errors);
        }
        catch (Exception ex)
        {
            return new ResultUploadResponse(0, 0, 1, [$"Error parsing CSV: {ex.Message}"]);
        }
    }

    public async Task<StudentAssessmentDto?> GetStudentAssessmentAsync(Guid assessmentId)
    {
        return await dbContext.StudentAssessments
            .Include(a => a.ExamTaker)
            .Include(a => a.Session)
            .Include(a => a.Term)
            .Include(a => a.ClassLevel)
            .Include(a => a.SubjectScores)
                .ThenInclude(s => s.Subject)
            .Where(a => a.Id == assessmentId)
            .Select(a => MapToDto(a))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<StudentAssessmentDto>> GetClassAssessmentsAsync(Guid sessionId, Guid termId, Guid classLevelId)
    {
        return await dbContext.StudentAssessments
            .Include(a => a.ExamTaker)
            .Include(a => a.Session)
            .Include(a => a.Term)
            .Include(a => a.ClassLevel)
            .Include(a => a.SubjectScores)
                .ThenInclude(s => s.Subject)
            .Where(a => a.SessionId == sessionId && a.TermId == termId && a.ClassLevelId == classLevelId)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentAssessmentDto>> GetParentChildrenResultsAsync(string parentUserId)
    {
        var studentIds = await dbContext.ParentStudentLinks
            .Where(l => l.ParentId == parentUserId)
            .Select(l => l.StudentId)
            .ToListAsync();

        return await dbContext.StudentAssessments
            .Include(a => a.ExamTaker)
            .Include(a => a.Session)
            .Include(a => a.Term)
            .Include(a => a.ClassLevel)
            .Include(a => a.SubjectScores)
                .ThenInclude(s => s.Subject)
            .Where(a => studentIds.Contains(a.ExamTakerId) && a.Status == ModerationStatus.Published)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(Guid assessmentId, ModerationStatus status)
    {
        var assessment = await dbContext.StudentAssessments.FindAsync(assessmentId);
        if (assessment != null)
        {
            assessment.Status = status;
            if (status == ModerationStatus.Published)
            {
                assessment.PublishedAt = DateTime.UtcNow;
            }
            await dbContext.SaveChangesAsync();
        }
    }

    private static StudentAssessmentDto MapToDto(StudentAssessmentEntity a)
    {
        return new StudentAssessmentDto(
            a.Id,
            a.ExamTakerId,
            a.ExamTaker?.FullName ?? "Unknown",
            a.ExamTaker?.AdmissionNumber ?? "N/A",
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
            )).ToList()
        );
    }
}
