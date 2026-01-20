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

        var configuredConn = app.Configuration.GetConnectionString("DefaultConnection") ?? "(none)";
        string connPreview;
        try
        {
            // redact password for logs
            connPreview = configuredConn.Replace("Password=", "Password=***");
        }
        catch
        {
            connPreview = "(invalid)";
        }

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
}
