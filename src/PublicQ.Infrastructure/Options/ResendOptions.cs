namespace PublicQ.Infrastructure.Options;

/// <summary>
/// Resend configuration model.
/// </summary>
public class ResendOptions
{
    /// <summary>
    /// API key for Resend service.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
