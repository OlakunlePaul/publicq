using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PublicQ.Domain.Models;
using PublicQ.Domain.Enums;
using PublicQ.Infrastructure.Persistence;
using PublicQ.Infrastructure.Persistence.Entities;
using PublicQ.Infrastructure.Persistence.Entities.Academic;
using PublicQ.Shared;
using PublicQ.Shared.Enums;

namespace PublicQ.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeds test data for end-to-end workflow verification.
/// </summary>
public static class TestDataSeeder
{
    public static async Task SeedTestDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        const string TestPassword = "TestPassword123!";

        // 1. Ensure Active Session exists
        var testSession = await dbContext.Sessions.FirstOrDefaultAsync(s => s.IsActive);
        if (testSession == null)
        {
            testSession = new SessionEntity
            {
                Id = Guid.NewGuid(),
                Name = "2025/2026 Academic Session",
                StartDate = DateTime.UtcNow.AddMonths(-3),
                EndDate = DateTime.UtcNow.AddMonths(9),
                IsActive = true
            };
            await dbContext.Sessions.AddAsync(testSession);
            await dbContext.SaveChangesAsync();
        }

        // 2. Ensure Active Term exists
        var testTerm = await dbContext.Terms.FirstOrDefaultAsync(t => t.IsActive && t.SessionId == testSession.Id);
        if (testTerm == null)
        {
            testTerm = new TermEntity
            {
                Id = Guid.NewGuid(),
                SessionId = testSession.Id,
                Name = "First Term",
                StartDate = DateTime.UtcNow.AddMonths(-3),
                EndDate = DateTime.UtcNow.AddMonths(1),
                IsActive = true
            };
            await dbContext.Terms.AddAsync(testTerm);
            await dbContext.SaveChangesAsync();
        }

        // 3. Ensure Class Level exists
        var testClass = await dbContext.ClassLevels.FirstOrDefaultAsync(cl => cl.Name == "JSS 1 Test");
        if (testClass == null)
        {
            testClass = new ClassLevelEntity
            {
                Id = Guid.NewGuid(),
                Name = "JSS 1 Test",
                SectionOrArm = "A",
                OrderIndex = 1
            };
            await dbContext.ClassLevels.AddAsync(testClass);
            await dbContext.SaveChangesAsync();
        }

        // 4. Ensure Subject exists and is linked
        var testSubject = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name == "English Language Test");
        if (testSubject == null)
        {
            testSubject = new SubjectEntity
            {
                Id = Guid.NewGuid(),
                Name = "English Language Test",
                Code = "ENG-TEST",
                DisplayOrder = 1
            };
            testSubject.ClassLevels.Add(testClass);
            await dbContext.Subjects.AddAsync(testSubject);
            await dbContext.SaveChangesAsync();
        }

        // 5. Create Test Teacher (Contributor)
        const string TeacherEmail = "teacher@test.local";
        var teacherUser = await userManager.FindByEmailAsync(TeacherEmail);
        if (teacherUser == null)
        {
            teacherUser = new ApplicationUser
            {
                FullName = "Test Teacher",
                UserName = TeacherEmail,
                Email = TeacherEmail,
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            await userManager.CreateAsync(teacherUser, TestPassword);
            await userManager.AddToRoleAsync(teacherUser, UserRolesNames.Contributor);
        }

        // 6. Create Test Student (Student role + StudentEntity)
        const string StudentEmail = "student@test.local";
        var studentUser = await userManager.FindByEmailAsync(StudentEmail);
        if (studentUser == null)
        {
            studentUser = new ApplicationUser
            {
                FullName = "Test Student",
                UserName = StudentEmail,
                Email = StudentEmail,
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            await userManager.CreateAsync(studentUser, TestPassword);
            await userManager.AddToRoleAsync(studentUser, UserRolesNames.Student);
        }

        var studentEntity = await dbContext.Students.FirstOrDefaultAsync(s => s.Email == StudentEmail);
        if (studentEntity == null)
        {
            studentEntity = new StudentEntity
            {
                Id = studentUser.Id,
                Email = StudentEmail,
                NormalizedEmail = StudentEmail.ToUpperInvariant(),
                FullName = "Test Student",
                AdmissionNumber = "T-STUD-001",
                CreatedAtUtc = DateTime.UtcNow
            };
            await dbContext.Students.AddAsync(studentEntity);
            await dbContext.SaveChangesAsync();
        }

        // 7. Ensure Student Enrollment (StudentAssessmentEntity)
        var studentEnrollment = await dbContext.StudentAssessments
            .FirstOrDefaultAsync(sa => sa.StudentId == studentEntity.Id && sa.SessionId == testSession.Id && sa.TermId == testTerm.Id);
        
        if (studentEnrollment == null)
        {
            studentEnrollment = new StudentAssessmentEntity
            {
                Id = Guid.NewGuid(),
                StudentId = studentEntity.Id,
                SessionId = testSession.Id,
                TermId = testTerm.Id,
                ClassLevelId = testClass.Id,
                Status = ModerationStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };
            await dbContext.StudentAssessments.AddAsync(studentEnrollment);
            await dbContext.SaveChangesAsync();
        }

        // 8. Create Test Parent
        const string ParentEmail = "parent@test.local";
        var parentUser = await userManager.FindByEmailAsync(ParentEmail);
        if (parentUser == null)
        {
            parentUser = new ApplicationUser
            {
                FullName = "Test Parent",
                UserName = ParentEmail,
                Email = ParentEmail,
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            await userManager.CreateAsync(parentUser, TestPassword);
            await userManager.AddToRoleAsync(parentUser, UserRolesNames.Parent);
        }

        // Link Parent to Student
        var parentLink = await dbContext.ParentStudentLinks
            .FirstOrDefaultAsync(l => l.ParentId == parentUser.Id && l.StudentId == studentEntity.Id);
        if (parentLink == null)
        {
            parentLink = new ParentStudentLinkEntity
            {
                Id = Guid.NewGuid(),
                ParentId = parentUser.Id,
                StudentId = studentEntity.Id,
                Relationship = "Father"
            };
            await dbContext.ParentStudentLinks.AddAsync(parentLink);
            await dbContext.SaveChangesAsync();
        }
    }
}
