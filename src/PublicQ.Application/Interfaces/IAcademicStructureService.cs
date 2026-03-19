using PublicQ.Application.Models;
using PublicQ.Application.Models.Academic;
using PublicQ.Domain.Enums;

namespace PublicQ.Application.Interfaces;

/// <summary>
/// Service for managing academic infrastructure (Sessions, Terms, Classes, Subjects).
/// </summary>
public interface IAcademicStructureService
{
    // Subjects
    Task<Response<IList<SubjectDto>, GenericOperationStatuses>> GetSubjectsAsync(CancellationToken cancellationToken = default);
    Task<Response<SubjectDto, GenericOperationStatuses>> CreateSubjectAsync(SubjectCreateDto dto, CancellationToken cancellationToken = default);
    Task<Response<SubjectDto, GenericOperationStatuses>> UpdateSubjectAsync(Guid id, SubjectCreateDto dto, CancellationToken cancellationToken = default);
    Task<Response<GenericOperationStatuses>> DeleteSubjectAsync(Guid id, CancellationToken cancellationToken = default);

    // Sessions
    Task<Response<IList<SessionDto>, GenericOperationStatuses>> GetSessionsAsync(CancellationToken cancellationToken = default);
    Task<Response<SessionDto, GenericOperationStatuses>> CreateSessionAsync(SessionCreateDto dto, CancellationToken cancellationToken = default);
    Task<Response<SessionDto, GenericOperationStatuses>> UpdateSessionAsync(Guid id, SessionCreateDto dto, CancellationToken cancellationToken = default);
    Task<Response<GenericOperationStatuses>> DeleteSessionAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Response<GenericOperationStatuses>> SetActiveSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    // Terms
    Task<Response<IList<TermDto>, GenericOperationStatuses>> GetTermsBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Response<TermDto, GenericOperationStatuses>> CreateTermAsync(TermCreateDto dto, CancellationToken cancellationToken = default);
    Task<Response<TermDto, GenericOperationStatuses>> UpdateTermAsync(Guid id, TermCreateDto dto, CancellationToken cancellationToken = default);
    Task<Response<GenericOperationStatuses>> DeleteTermAsync(Guid id, CancellationToken cancellationToken = default);

    // Class Levels
    Task<Response<IList<ClassLevelDto>, GenericOperationStatuses>> GetClassLevelsAsync(CancellationToken cancellationToken = default);
    Task<Response<ClassLevelDto, GenericOperationStatuses>> CreateClassLevelAsync(ClassLevelCreateDto dto, CancellationToken cancellationToken = default);
    Task<Response<ClassLevelDto, GenericOperationStatuses>> UpdateClassLevelAsync(Guid id, ClassLevelCreateDto dto, CancellationToken cancellationToken = default);
    Task<Response<GenericOperationStatuses>> DeleteClassLevelAsync(Guid id, CancellationToken cancellationToken = default);
}
