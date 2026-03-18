using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PublicQ.Infrastructure.Authorization;

/// <summary>
/// Custom policy provider to handle dynamic permission policies (e.g., "Permission:Users.View").
/// </summary>
public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if the policy name starts with our prefix
        if (policyName.StartsWith("Permission:", StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName.Substring("Permission:".Length);
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirement(permission));
            return policy.Build();
        }

        // Fallback to default provider for other policies (like roles)
        return await base.GetPolicyAsync(policyName);
    }
}
