namespace PublicQ.Infrastructure.Options;

public class S3Options
{
    public bool Enabled { get; set; } = false;
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? BucketName { get; set; }
    public string? Endpoint { get; set; }
    public string? ServiceUrl { get; set; }
    public string? Region { get; set; }
}
