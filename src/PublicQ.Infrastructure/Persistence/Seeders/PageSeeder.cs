using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models.Pages;
using PublicQ.Shared.Models;

namespace PublicQ.Infrastructure.Persistence.Seeders;

public static class PageSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var pageService = scope.ServiceProvider.GetRequiredService<IPageService>();
        
        // Seed Contact Page
        var contactPageResponse = await pageService.GetContactPageAsync(default);
        if (!contactPageResponse.IsSuccess)
        {
            var defaultContactPage = new PageDto
            {
                Type = PageType.Contact,
                Title = "Contact Us",
                Body = "Get in touch with us for any inquiries or support.",
                JsonData = "{\"Email\":\"support@examnova.app\",\"Phone\":\"+1234567890\",\"Address\":\"123 School Lane, Education City\"}"
            };
            
            await pageService.SetOrUpdateAsync(defaultContactPage, "System", default);
        }
    }
}
