namespace Reminders.Api.Extensions;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Polly;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;

public static class DatabaseExtensions
{
    public static WebApplication EnsureDatabaseAvailable(this WebApplication app)
    {
        var loggerFactory = app.Services.GetService<ILoggerFactory>();
        var logger = loggerFactory?.CreateLogger("DatabaseInit");

        var maxRetryAttempts = app.Configuration.GetValue<int?>("DatabaseRetry:MaxAttempts") ?? 5;
        var baseSeconds = app.Configuration.GetValue<int?>("DatabaseRetry:BaseSeconds") ?? 2;

        // Get redacted connection string for logging (password never stored)
        var connPreview = GetRedactedConnectionString(app.Configuration);

        var policy = Policy.Handle<Exception>()
            .WaitAndRetry(maxRetryAttempts, retryAttempt =>
            {
                var jitter = TimeSpan.FromMilliseconds(new Random().Next(0, 1000));
                return TimeSpan.FromSeconds(Math.Pow(baseSeconds, retryAttempt)) + jitter;
            }, (exception, timeSpan, retryCount, context) =>
            {
                logger?.LogWarning(exception, "Database connectivity attempt {RetryCount} failed for {ConnectionPreview}. Next retry in {Delay}.", retryCount, connPreview, timeSpan);
            });

        try
        {
            policy.Execute(() =>
            {
                var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
                using var scope = scopedFactory?.CreateScope();

                var db = scope?.ServiceProvider.GetService<RemindersContext>();
                if (db is null)
                {
                    throw new InvalidOperationException("RemindersContext not registered.");
                }

                var conn = db.Database.GetDbConnection();
                conn.Open();
                conn.Close();

                logger?.LogInformation("Database connection verified.");
            });
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Database connectivity could not be established after {Attempts} attempts to {ConnectionPreview}. Verify the database is running and the connection settings (env/.env). Startup will continue; migrations may fail.", maxRetryAttempts, connPreview);
        }

        return app;
    }

    private static string GetRedactedConnectionString(IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection");
        return RedactPassword(conn ?? "(none)");
    }

    private static string RedactPassword(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return "(none)";

        try
        {
            // Use regex to redact password value
            return System.Text.RegularExpressions.Regex.Replace(
                connectionString,
                @"(Password|Pwd)\s*=\s*[^;]*",
                "$1=***",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
        catch
        {
            return "(invalid)";
        }
    }
}
