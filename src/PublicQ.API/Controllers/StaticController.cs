using Microsoft.AspNetCore.Mvc;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Shared;

namespace PublicQ.API.Controllers;

[ApiController]
[Route("api/static")]
public class StaticController : ControllerBase
{
    private readonly IStorageService _storageService;

    public StaticController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpGet("{*path}")]
    public async Task<IActionResult> GetStaticFile(string path, CancellationToken cancellationToken)
    {
        // Normalize path: strip leading slash if present
        if (path.StartsWith("/"))
        {
            path = path.Substring(1);
        }

        // The path usually includes the category, e.g., 'branding/logo.png'
        // But the relative path might have been stored as 'static/branding/logo.png' 
        // because of the FileStorageService prefix.
        // We strip 'static/' if it exists at the start.
        if (path.StartsWith("static/", StringComparison.OrdinalIgnoreCase))
        {
            path = path.Substring(7);
        }

        var itemName = Path.GetFileName(path);
        var relativePath = Path.GetDirectoryName(path)?.Replace('\\', '/');

        var result = await _storageService.GetAsync(itemName, cancellationToken, relativePath);

        if (!result.IsSuccess || result.Data == null)
        {
            return NotFound();
        }

        var contentType = GetContentType(itemName);
        return File(result.Data.Content, contentType);
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
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}
