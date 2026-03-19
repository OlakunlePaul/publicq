using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Application.Models.Academic;
using PublicQ.Domain.Enums;
using PublicQ.Infrastructure.Persistence;
using PublicQ.Infrastructure.Persistence.Entities.Academic;

namespace PublicQ.Infrastructure.Services;

public class AcademicStructureService(
    ApplicationDbContext dbContext) : IAcademicStructureService
{
    public async Task<Response<IList<SubjectDto>, GenericOperationStatuses>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        var subjects = await dbContext.Subjects
            .AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .Select(s => new SubjectDto(s.Id, s.Name, s.Code, s.DisplayOrder))
            .ToListAsync(cancellationToken);

        return Response<IList<SubjectDto>, GenericOperationStatuses>.Success(
            subjects, GenericOperationStatuses.Completed, "Subjects retrieved successfully.");
    }

    public async Task<Response<SubjectDto, GenericOperationStatuses>> CreateSubjectAsync(SubjectCreateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Subjects.AnyAsync(s => s.Name == dto.Name, cancellationToken);
        if (existing)
        {
            return Response<SubjectDto, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Conflict, "A subject with this name already exists.");
        }

        var subject = new SubjectEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            DisplayOrder = dto.DisplayOrder
        };

        dbContext.Subjects.Add(subject);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Response<SubjectDto, GenericOperationStatuses>.Success(
            new SubjectDto(subject.Id, subject.Name, subject.Code, subject.DisplayOrder),
            GenericOperationStatuses.Completed, "Subject created successfully.");
    }

    public async Task<Response<IList<SessionDto>, GenericOperationStatuses>> GetSessionsAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await dbContext.Sessions
            .AsNoTracking()
            .OrderByDescending(s => s.StartDate)
            .Select(s => new SessionDto(s.Id, s.Name, s.StartDate, s.EndDate, s.IsActive))
            .ToListAsync(cancellationToken);

        return Response<IList<SessionDto>, GenericOperationStatuses>.Success(
            sessions, GenericOperationStatuses.Completed, "Sessions retrieved successfully.");
    }

    public async Task<Response<SessionDto, GenericOperationStatuses>> CreateSessionAsync(SessionCreateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Sessions.AnyAsync(s => s.Name == dto.Name, cancellationToken);
        if (existing)
        {
            return Response<SessionDto, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Conflict, "A session with this name already exists.");
        }

        var session = new SessionEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = dto.IsActive
        };

        if (session.IsActive)
        {
            var activeSessions = await dbContext.Sessions.Where(s => s.IsActive).ToListAsync(cancellationToken);
            foreach (var acts in activeSessions) acts.IsActive = false;
        }

        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Response<SessionDto, GenericOperationStatuses>.Success(
            new SessionDto(session.Id, session.Name, session.StartDate, session.EndDate, session.IsActive),
            GenericOperationStatuses.Completed, "Session created successfully.");
    }

    public async Task<Response<GenericOperationStatuses>> SetActiveSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.Sessions.FindAsync(new object[] { sessionId }, cancellationToken);
        if (session == null)
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Session not found.");

        var allSessions = await dbContext.Sessions.ToListAsync(cancellationToken);
        foreach (var s in allSessions)
        {
            s.IsActive = (s.Id == sessionId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Active session updated.");
    }

    public async Task<Response<IList<TermDto>, GenericOperationStatuses>> GetTermsBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var terms = await dbContext.Terms
            .AsNoTracking()
            .Include(t => t.Session)
            .Where(t => t.SessionId == sessionId)
            .OrderBy(t => t.StartDate)
            .Select(t => new TermDto(t.Id, t.SessionId, t.Session.Name, t.Name, t.StartDate, t.EndDate, t.NextTermBegins, t.IsActive))
            .ToListAsync(cancellationToken);

        return Response<IList<TermDto>, GenericOperationStatuses>.Success(
            terms, GenericOperationStatuses.Completed, "Terms retrieved successfully.");
    }

    public async Task<Response<TermDto, GenericOperationStatuses>> CreateTermAsync(TermCreateDto dto, CancellationToken cancellationToken = default)
    {
        var sessionExists = await dbContext.Sessions.AnyAsync(s => s.Id == dto.SessionId, cancellationToken);
        if (!sessionExists)
            return Response<TermDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Session not found.");

        var term = new TermEntity
        {
            Id = Guid.NewGuid(),
            SessionId = dto.SessionId,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            NextTermBegins = dto.NextTermBegins,
            IsActive = dto.IsActive
        };

        if (term.IsActive)
        {
            var activeTerms = await dbContext.Terms.Where(t => t.SessionId == dto.SessionId && t.IsActive).ToListAsync(cancellationToken);
            foreach (var actt in activeTerms) actt.IsActive = false;
        }

        dbContext.Terms.Add(term);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Response<TermDto, GenericOperationStatuses>.Success(
            new TermDto(term.Id, term.SessionId, "Session", term.Name, term.StartDate, term.EndDate, term.NextTermBegins, term.IsActive),
            GenericOperationStatuses.Completed, "Term created successfully.");
    }

    public async Task<Response<IList<ClassLevelDto>, GenericOperationStatuses>> GetClassLevelsAsync(CancellationToken cancellationToken = default)
    {
        var classLevels = await dbContext.ClassLevels
            .AsNoTracking()
            .OrderBy(c => c.OrderIndex)
            .ThenBy(c => c.Name)
            .Select(c => new ClassLevelDto(c.Id, c.Name, c.SectionOrArm, c.OrderIndex))
            .ToListAsync(cancellationToken);

        return Response<IList<ClassLevelDto>, GenericOperationStatuses>.Success(
            classLevels, GenericOperationStatuses.Completed, "Class levels retrieved successfully.");
    }

    public async Task<Response<ClassLevelDto, GenericOperationStatuses>> CreateClassLevelAsync(ClassLevelCreateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.ClassLevels.AnyAsync(c => c.Name == dto.Name && c.SectionOrArm == dto.SectionOrArm, cancellationToken);
        if (existing)
        {
            return Response<ClassLevelDto, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Conflict, "A class level with this name and section already exists.");
        }

        var classLevel = new ClassLevelEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            SectionOrArm = dto.SectionOrArm,
            OrderIndex = dto.OrderIndex
        };

        dbContext.ClassLevels.Add(classLevel);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Response<ClassLevelDto, GenericOperationStatuses>.Success(
            new ClassLevelDto(classLevel.Id, classLevel.Name, classLevel.SectionOrArm, classLevel.OrderIndex),
            GenericOperationStatuses.Completed, "Class level created successfully.");
    }

    public async Task<Response<SubjectDto, GenericOperationStatuses>> UpdateSubjectAsync(Guid id, SubjectCreateDto dto, CancellationToken cancellationToken = default)
    {
        var subject = await dbContext.Subjects.FindAsync(new object[] { id }, cancellationToken);
        if (subject == null) return Response<SubjectDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Subject not found.");

        subject.Name = dto.Name;
        subject.Code = dto.Code;
        subject.DisplayOrder = dto.DisplayOrder;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<SubjectDto, GenericOperationStatuses>.Success(
            new SubjectDto(subject.Id, subject.Name, subject.Code, subject.DisplayOrder),
            GenericOperationStatuses.Completed, "Subject updated successfully.");
    }

    public async Task<Response<GenericOperationStatuses>> DeleteSubjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subject = await dbContext.Subjects.FindAsync(new object[] { id }, cancellationToken);
        if (subject == null) return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Subject not found.");

        dbContext.Subjects.Remove(subject);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Subject deleted successfully.");
    }

    public async Task<Response<SessionDto, GenericOperationStatuses>> UpdateSessionAsync(Guid id, SessionCreateDto dto, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.Sessions.FindAsync(new object[] { id }, cancellationToken);
        if (session == null) return Response<SessionDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Session not found.");

        session.Name = dto.Name;
        session.StartDate = dto.StartDate;
        session.EndDate = dto.EndDate;

        if (dto.IsActive && !session.IsActive)
        {
            var activeSessions = await dbContext.Sessions.Where(s => s.IsActive).ToListAsync(cancellationToken);
            foreach (var acts in activeSessions) acts.IsActive = false;
        }
        session.IsActive = dto.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<SessionDto, GenericOperationStatuses>.Success(
            new SessionDto(session.Id, session.Name, session.StartDate, session.EndDate, session.IsActive),
            GenericOperationStatuses.Completed, "Session updated successfully.");
    }

    public async Task<Response<GenericOperationStatuses>> DeleteSessionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.Sessions.Include(s => s.Terms).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (session == null) return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Session not found.");

        if (session.Terms.Any())
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict, "Cannot delete session with existing terms. Delete terms first.");

        dbContext.Sessions.Remove(session);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Session deleted successfully.");
    }

    public async Task<Response<TermDto, GenericOperationStatuses>> UpdateTermAsync(Guid id, TermCreateDto dto, CancellationToken cancellationToken = default)
    {
        var term = await dbContext.Terms.FindAsync(new object[] { id }, cancellationToken);
        if (term == null) return Response<TermDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Term not found.");

        term.Name = dto.Name;
        term.StartDate = dto.StartDate;
        term.EndDate = dto.EndDate;
        term.NextTermBegins = dto.NextTermBegins;

        if (dto.IsActive && !term.IsActive)
        {
            var activeTerms = await dbContext.Terms.Where(t => t.SessionId == term.SessionId && t.IsActive).ToListAsync(cancellationToken);
            foreach (var actt in activeTerms) actt.IsActive = false;
        }
        term.IsActive = dto.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<TermDto, GenericOperationStatuses>.Success(
            new TermDto(term.Id, term.SessionId, "Session", term.Name, term.StartDate, term.EndDate, term.NextTermBegins, term.IsActive),
            GenericOperationStatuses.Completed, "Term updated successfully.");
    }

    public async Task<Response<GenericOperationStatuses>> DeleteTermAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var term = await dbContext.Terms.FindAsync(new object[] { id }, cancellationToken);
        if (term == null) return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Term not found.");

        dbContext.Terms.Remove(term);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Term deleted successfully.");
    }

    public async Task<Response<ClassLevelDto, GenericOperationStatuses>> UpdateClassLevelAsync(Guid id, ClassLevelCreateDto dto, CancellationToken cancellationToken = default)
    {
        var classLevel = await dbContext.ClassLevels.FindAsync(new object[] { id }, cancellationToken);
        if (classLevel == null) return Response<ClassLevelDto, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Class level not found.");

        classLevel.Name = dto.Name;
        classLevel.SectionOrArm = dto.SectionOrArm;
        classLevel.OrderIndex = dto.OrderIndex;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<ClassLevelDto, GenericOperationStatuses>.Success(
            new ClassLevelDto(classLevel.Id, classLevel.Name, classLevel.SectionOrArm, classLevel.OrderIndex),
            GenericOperationStatuses.Completed, "Class level updated successfully.");
    }

    public async Task<Response<GenericOperationStatuses>> DeleteClassLevelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var classLevel = await dbContext.ClassLevels.FindAsync(new object[] { id }, cancellationToken);
        if (classLevel == null) return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "Class level not found.");

        dbContext.ClassLevels.Remove(classLevel);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "Class level deleted successfully.");
    }
}
