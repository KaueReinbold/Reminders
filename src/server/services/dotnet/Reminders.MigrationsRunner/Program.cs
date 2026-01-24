using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Reminders.Application.Enumerables;
using Reminders.Application.Extensions;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;

// Create builder for configuration and DI
var builder = WebApplication.CreateBuilder(args);

// Add configuration from appsettings.json and environment variables
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.MigrationRunner.json", optional: false)
    .AddEnvironmentVariables();

// Register services (reuse from API project)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var provider = builder.Configuration.GetValue<SupportedDatabases?>("DatabaseProvider") ?? SupportedDatabases.Postgres;

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERROR: Connection string not configured");
    Environment.Exit(1);
}

builder.Services.RegisterApplicationServices(connectionString, provider);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Shared state for health endpoint
var migrationStatus = MigrationStatus.Pending;
string? errorMessage = null;
var startTime = DateTime.UtcNow;
var endTime = DateTime.UtcNow;

// Migration execution task
var migrationTask = Task.Run(async () =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        migrationStatus = MigrationStatus.Running;
        startTime = DateTime.UtcNow;
        
        // Redact password from connection string for logging
        var redactedConnectionString = RedactConnectionString(connectionString);
        logger.LogInformation("Migration runner starting with provider: {Provider}, connection: {Connection}", 
            provider, redactedConnectionString);

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RemindersContext>();

        // Retry policy configuration from appsettings
        var maxRetries = builder.Configuration.GetValue<int>("MigrationRunner:MaxRetryAttempts", 5);
        var baseDelay = builder.Configuration.GetValue<int>("MigrationRunner:RetryBaseDelaySeconds", 2);

        var policy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(maxRetries, retryAttempt =>
            {
                // Exponential backoff with jitter
                var jitter = TimeSpan.FromMilliseconds(new Random().Next(0, 1000));
                return TimeSpan.FromSeconds(Math.Pow(baseDelay, retryAttempt)) + jitter;
            }, (exception, timeSpan, retryCount, context) =>
            {
                logger.LogWarning(exception, "Migration attempt {RetryCount} of {MaxRetries} failed. Waiting {Delay} before next attempt.", 
                    retryCount, maxRetries, timeSpan);
            });

        await policy.ExecuteAsync(async () =>
        {
            logger.LogInformation("Applying database migrations...");

            // Apply provider-specific migrations (reuse existing logic)
            try
            {
                // Get migrations assembly and migrator
                var migrationsAssembly = scope.ServiceProvider.GetService<IMigrationsAssembly>()
                                         ?? db.GetInfrastructure().GetService<IMigrationsAssembly>();
                var migrator = scope.ServiceProvider.GetService<IMigrator>()
                              ?? db.GetInfrastructure().GetService<IMigrator>();

                if (migrationsAssembly is not null && migrator is not null)
                {
                    // Log discovered migrations
                    var allMigrations = migrationsAssembly.Migrations.ToList();
                    logger.LogInformation("Discovered {Count} total migrations", allMigrations.Count);
                    
                    foreach (var migration in allMigrations)
                    {
                        logger.LogInformation("[{Timestamp}] Discovered migration: {MigrationId} in namespace {Namespace}", 
                            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), 
                            migration.Key, 
                            migration.Value?.Namespace ?? "Unknown");
                    }

                    // Filter provider-specific migrations
                    var targetMigrations = provider == SupportedDatabases.Postgres
                        ? migrationsAssembly.Migrations
                            .Where(kv => (kv.Value?.Namespace ?? string.Empty).Contains(".Postgres."))
                            .Select(kv => kv.Key)
                            .OrderBy(id => id)
                            .ToList()
                        : migrationsAssembly.Migrations
                            .Where(kv => (kv.Value?.Namespace ?? string.Empty).Contains(".SqlServer."))
                            .Select(kv => kv.Key)
                            .OrderBy(id => id)
                            .ToList();

                    if (targetMigrations.Any())
                    {
                        var target = targetMigrations.Last();
                        logger.LogInformation("Applying {Provider} migrations up to {Migration} ({Count} migrations)", 
                            provider, target, targetMigrations.Count);
                        
                        // Apply migrations with per-migration logging
                        await migrator.MigrateAsync(target);
                        
                        logger.LogInformation("[{Timestamp}] Successfully applied {Count} {Provider} migrations", 
                            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                            targetMigrations.Count, 
                            provider);
                    }
                    else
                    {
                        logger.LogInformation("No provider-specific migrations found, applying all migrations");
                        await db.Database.MigrateAsync();
                    }
                }
                else
                {
                    logger.LogInformation("Applying migrations using default strategy");
                    await db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                // Fallback to default migrate if provider-specific logic fails
                logger.LogWarning(ex, "Provider-specific migration failed, falling back to default strategy");
                await db.Database.MigrateAsync();
            }

            endTime = DateTime.UtcNow;
            var duration = (endTime - startTime).TotalSeconds;
            logger.LogInformation("Database migrations applied successfully in {Duration:F2} seconds", duration);
        });

        migrationStatus = MigrationStatus.Completed;
    }
    catch (Exception ex)
    {
        migrationStatus = MigrationStatus.Failed;
        errorMessage = ex.Message;
        endTime = DateTime.UtcNow;
        logger.LogError(ex, "Migration execution failed after all retry attempts");
    }
});

// Health endpoint
app.MapGet("/healthz", () =>
{
    var duration = (endTime - startTime).TotalSeconds;
    
    return migrationStatus switch
    {
        MigrationStatus.Completed => Results.Ok(new
        {
            status = "completed",
            message = $"Successfully applied migrations in {duration:F1} seconds",
            timestamp = DateTime.UtcNow.ToString("o")
        }),
        MigrationStatus.Failed => Results.Problem(
            statusCode: 500,
            detail: $"Migration execution failed: {errorMessage}",
            title: "Migration Failed"),
        MigrationStatus.Running => Results.Problem(
            statusCode: 500,
            detail: "Migrations in progress",
            title: "Migrations Running"),
        _ => Results.Problem(
            statusCode: 500,
            detail: "Migrations pending - execution not yet started",
            title: "Migrations Pending")
    };
});

// Start HTTP server in background
var serverTask = app.RunAsync();

// Wait for migrations to complete
await migrationTask;

// Graceful shutdown
await app.StopAsync();

// Exit with appropriate code
Environment.Exit(migrationStatus == MigrationStatus.Completed ? 0 : 1);

// Helper method to redact passwords from connection strings
static string RedactConnectionString(string connectionString)
{
    var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
    var redacted = parts.Select(part =>
    {
        var keyValue = part.Split('=', 2);
        if (keyValue.Length == 2 && 
            (keyValue[0].Trim().Equals("Password", StringComparison.OrdinalIgnoreCase) ||
             keyValue[0].Trim().Equals("Pwd", StringComparison.OrdinalIgnoreCase)))
        {
            return $"{keyValue[0]}=***";
        }
        return part;
    });
    return string.Join(";", redacted);
}

// MigrationStatus enum for tracking execution state
enum MigrationStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
