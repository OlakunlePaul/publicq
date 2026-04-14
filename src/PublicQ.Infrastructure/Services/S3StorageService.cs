using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Infrastructure.Options;
using PublicQ.Shared;
using PublicQ.Shared.Models;

namespace PublicQ.Infrastructure.Services;

public class S3StorageService : IStorageService
{
    private readonly IOptionsMonitor<S3Options> _options;
    private readonly ILogger<S3StorageService> _logger;
    private readonly IAmazonS3 _s3Client;

    public S3StorageService(IOptionsMonitor<S3Options> options, ILogger<S3StorageService> logger)
    {
        _options = options;
        _logger = logger;
        
        var config = options.CurrentValue;
        
        var s3Config = new AmazonS3Config();
        if (!string.IsNullOrEmpty(config.ServiceUrl))
        {
            s3Config.ServiceURL = config.ServiceUrl;
        }
        else if (!string.IsNullOrEmpty(config.Endpoint))
        {
            s3Config.ServiceURL = config.Endpoint;
        }
        
        if (!string.IsNullOrEmpty(config.Region))
        {
             s3Config.RegionEndpoint = RegionEndpoint.GetBySystemName(config.Region);
        }

        _s3Client = new AmazonS3Client(config.AccessKey, config.SecretKey, s3Config);
    }

    public async Task<Response<string, GenericOperationStatuses>> SaveAsync(StorageItem storageItem, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(storageItem, nameof(storageItem));
        Guard.AgainstNullOrWhiteSpace(storageItem.Name, nameof(storageItem.Name));
        Guard.AgainstNull(storageItem.Content, nameof(storageItem.Content));

        var key = storageItem.Name;
        if (!string.IsNullOrEmpty(storageItem.RelativePath))
        {
            key = $"{storageItem.RelativePath.TrimEnd('/')}/{storageItem.Name}";
        }

        try
        {
            using var stream = new MemoryStream(storageItem.Content);
            var request = new PutObjectRequest
            {
                BucketName = _options.CurrentValue.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = GetContentType(storageItem.Name)
            };

            await _s3Client.PutObjectAsync(request, cancellationToken);
            
            _logger.LogInformation("Successfully uploaded {Key} to S3 bucket {Bucket}", key, _options.CurrentValue.BucketName);
            
            // Return the key prefixed with 'static/' to trigger our proxy controller
            var relativeWebPath = $"static/{key}";
            return Response<string, GenericOperationStatuses>.Success(relativeWebPath, GenericOperationStatuses.Completed, "File saved to S3.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload {Key} to S3", key);
            return Response<string, GenericOperationStatuses>.Failure(GenericOperationStatuses.Failed, $"S3 Upload failed: {ex.Message}");
        }
    }

    public async Task<Response<StorageItem, GenericOperationStatuses>> GetAsync(string itemName, CancellationToken cancellationToken, string? relativePath = null)
    {
        var key = itemName;
        if (!string.IsNullOrEmpty(relativePath))
        {
            key = $"{relativePath.TrimEnd('/')}/{itemName}";
        }

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.CurrentValue.BucketName,
                Key = key
            };

            using var response = await _s3Client.GetObjectAsync(request, cancellationToken);
            using var ms = new MemoryStream();
            await response.ResponseStream.CopyToAsync(ms, cancellationToken);

            return Response<StorageItem, GenericOperationStatuses>.Success(new StorageItem
            {
                Name = itemName,
                Content = ms.ToArray()
            }, GenericOperationStatuses.Completed);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Response<StorageItem, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound, "File not found in S3.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve {Key} from S3", key);
            return Response<StorageItem, GenericOperationStatuses>.Failure(GenericOperationStatuses.Failed, $"S3 Retrieve failed: {ex.Message}");
        }
    }

    public async Task<Response<GenericOperationStatuses>> DeleteAsync(string filePath, CancellationToken cancellationToken, string? relativePath = null)
    {
        var key = filePath;
        if (!string.IsNullOrEmpty(relativePath))
        {
            key = $"{relativePath.TrimEnd('/')}/{filePath}";
        }

        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _options.CurrentValue.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
            return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed, "File deleted from S3.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete {Key} from S3", key);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.Failed, $"S3 Delete failed: {ex.Message}");
        }
    }

    private string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}
