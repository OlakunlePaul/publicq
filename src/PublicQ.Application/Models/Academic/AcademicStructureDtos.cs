namespace PublicQ.Application.Models.Academic;

public record SubjectDto(Guid Id, string Name, string? Code, int DisplayOrder);
public record SubjectCreateDto(string Name, string? Code, int DisplayOrder = 0);

public record SessionDto(Guid Id, string Name, DateTime? StartDate, DateTime? EndDate, bool IsActive);
public record SessionCreateDto(string Name, DateTime? StartDate, DateTime? EndDate, bool IsActive);

public record TermDto(Guid Id, Guid SessionId, string SessionName, string Name, DateTime? StartDate, DateTime? EndDate, DateTime? NextTermBegins, bool IsActive);
public record TermCreateDto(Guid SessionId, string Name, DateTime? StartDate, DateTime? EndDate, DateTime? NextTermBegins, bool IsActive);

public record ClassLevelDto(Guid Id, string Name, string? SectionOrArm, int OrderIndex);
public record ClassLevelCreateDto(string Name, string? SectionOrArm, int OrderIndex = 0);
