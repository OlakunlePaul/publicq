using System.Security.Claims;
using PublicQ.Domain.Enums;

namespace PublicQ.Shared;

/// <summary>
/// User claim parser helper methods.
/// </summary>
public static class UserClaimParser
{
    /// <summary>
    /// Gets user display name in format "Name (Email)" from claims.
    /// </summary>
    /// <param name="claims">Claims array</param>
    /// <returns>Returns user display name in format "Name (Email)" from claims. If either claim is missing, returns "Unknown".</returns>
    public static string GetUserDisplayName(IEnumerable<Claim> claims)
    {
        var enumerable = claims as Claim[] ?? claims.ToArray();
        
        var nameClaim = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        var emailClaim = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        
        if (nameClaim?.Value == null || emailClaim?.Value == null)
        {
            return "Unknown";
        }
        
        return $"{nameClaim.Value} ({emailClaim.Value})";
    }

    /// <summary>
    /// Gets user ID from claims.
    /// </summary>
    /// <param name="claims">Claims array</param>
    /// <returns>Returns user ID from claims or null if not found.</returns>
    public static string? GetUserId(IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Checks if user has Administrator role from claims.
    /// </summary>
    /// <param name="claims">Claims array</param>
    /// <returns>Returns true if user has Administrator role, otherwise false.</returns>
    public static bool IsAdministrator(IEnumerable<Claim> claims)
    {
        return claims.Any(c => c.Type == ClaimTypes.Role && c.Value == nameof(UserRole.Administrator));
    }
}