using AspNetCoreRateLimit;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using PublicQ.API.Middleware;
using PublicQ.Infrastructure;
using PublicQ.Infrastructure.Options;
using PublicQ.Infrastructure.Persistence;
using PublicQ.Infrastructure.Persistence.Seeders;
using PublicQ.Shared;
using ServiceRegistration = PublicQ.API.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS for split deployment (e.g. Vercel frontend + Railway backend)
var corsOrigins = builder.Configuration["CORS_ORIGINS"]?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
if (corsOrigins.Length > 0)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
}

#region add database custom provider

// Temp context to read DB settings
var connectionString = builder.Configuration.GetConnectionString(Constants.DbDefaultConnectionString);
var isSqlite = connectionString?.Contains("Data Source") == true || connectionString?.EndsWith(".db") == true;

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
if (isSqlite)
{
    optionsBuilder.UseSqlite(connectionString);
}
else
{
    optionsBuilder.UseNpgsql(connectionString);
}

var configContext = new ApplicationDbContext(optionsBuilder.Options);

try
{
    var rawConnectionString = builder.Configuration.GetConnectionString(Constants.DbDefaultConnectionString) ?? "NULL";
    var safeConnectionString = System.Text.RegularExpressions.Regex.Replace(rawConnectionString, @"Password=[^;]*", "Password=***");
    Console.WriteLine($"[DEBUG] Resolved Database Connection String (Password Hidden): '{safeConnectionString}'");

    Console.WriteLine("Executing automatic database migration step...");
    configContext.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine($"Automatic migration failed: {ex.Message}. It usually resolves itself on retries or check your Connection String.");
}

// Create the provider instance
var entityProvider = new EntityConfigurationProvider(configContext);

// Register the provider in DI
builder.Services.AddSingleton(entityProvider);

// Add the provider to the configuration system
((IConfigurationBuilder)builder.Configuration).Add(new EntityConfigurationSource(entityProvider));

#endregion

// Register other services
ServiceRegistration.AddApiServices(builder.Services, builder.Configuration);

// Now build
var app = builder.Build();

// Let's seed the database with available User Roles
try
{
    using (var scope = app.Services.CreateScope())
    {
        await UserRoleSeeder.SeedRolesAndDataAsync(scope.ServiceProvider);
        
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        await permissionService.SeedPermissionsAsync();
    }
    Console.WriteLine("Database seeding completed successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"[Warning] Database seeding failed: {ex.Message}. The app will still start, but roles may need to be seeded manually or on next restart.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS if configured
if (corsOrigins.Length > 0)
{
    app.UseCors();
}

// Configure static files serving for React app
app.UseDefaultFiles(); // This will serve index.html for requests to "/"
app.UseStaticFiles(); // This serves files from wwwroot

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseMiddleware<McpApiKeyMiddleware>();
app.MapMcp(Constants.McpRoutePrefix).AllowAnonymous(); // Middleware handles authentication/authorization

app.MapControllers();

// Configure static file serving for uploads
var fileStorageOptions = app.Services.GetRequiredService<IOptions<FileStorageOptions>>().Value;
var staticContentPath = fileStorageOptions.StaticContentPath ?? "static";
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), staticContentPath);
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = $"/{staticContentPath}"
});

// This maps all other than /api and /mcp requests to the React app when it runs in dev
if (app.Environment.IsDevelopment())
{
    app.MapWhen(ctx => !ctx.Request.Path.StartsWithSegments($"/{Constants.ControllerRoutePrefix}") 
                    && !ctx.Request.Path.StartsWithSegments($"/{Constants.McpRoutePrefix}"),
        spaApp =>
        {
            spaApp.UseSpa(spa =>
            {
                spa.Options.SourcePath = "../../client";
                spa.UseReactDevelopmentServer(npmScript: "start");
                spa.Options.StartupTimeout = TimeSpan.FromSeconds(60);
            });
        }
    );
}
else
{
    // In production, serve the React app for any route that doesn't start with /api
    app.MapFallbackToFile("/index.html");
}

// Enable IP Rate Limiting for API routes only
// PWA path is not prefixed with /api, so it won't be rate-limited
app.UseWhen(context => context.Request.Path.StartsWithSegments($"/{Constants.ControllerRoutePrefix}"), apiApp =>
{
    apiApp.UseIpRateLimiting();
});

app.Run();