using PublicQ.Shared;
using PublicQ.Domain.Enums;

namespace PublicQ.API.Models.Requests;

/// <summary>
/// Search user request model.
/// </summary>
public class SearchUserRequest
{
    /// <summary>
    /// Email part to search for.
    /// </summary>
    public string EmailPart { get; set; } = string.Empty;
    
    /// <summary>
    /// User ID part to search for.
    /// </summary>
    public string IdPart { get; set; } = string.Empty;

    /// <summary>
    /// PageSize
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// PageNumber
    /// </summary>
    public int PageNumber { get; set; } = 0;

    /// <summary>
    /// Role to filter by.
    /// </summary>
    public UserRole? Role { get; set; }
}