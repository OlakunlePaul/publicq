using PublicQ.Shared;
using Microsoft.AspNetCore.Mvc;
using PublicQ.Domain.Enums;

namespace PublicQ.API.Models.Requests;

/// <summary>
/// Get users' request model.
/// </summary>
public class GetPaginatedEntitiesRequest
{
    /// <summary>
    /// PageSize
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// PageNumber
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Role to filter by.
    /// </summary>
    public UserRole? Role { get; set; }
}