namespace Reminders.Application.Extensions;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

[ExcludeFromCodeCoverage]
public static class ApplicationConfiguration
{
    public static IServiceCollection RegisterApplicationServices(
        this IServiceCollection services,
        string? connectionString,
        SupportedDatabases supportedDatabases = SupportedDatabases.SqlServer)
    {
        services
            .AddSingleton(AutoMapperConfiguration.CreateMapper());

        if (connectionString is not null)
        {
            if (supportedDatabases == SupportedDatabases.Postgres)
                services.RegisterDataServicesPostgres(connectionString);
            else
                services.RegisterDataServicesSqlServer(connectionString);
        }
        else
        {
            throw new ArgumentException("Connection string is not provided.");
        }

        services.AddScoped<IRemindersService, RemindersService>();
        services.AddScoped<IRemindersBlockchainService, RemindersBlockchainService>();

        return services;
    }

    public static IServiceCollection AddApplicationValidations(
        this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddTransient<IValidator<ReminderViewModel>, ReminderViewModelValidator>();

        return services;
    }

    public static WebApplication MigrateRemindersDatabase(
       this WebApplication app)
    {
        // Configure retry policy: exponential backoff with jitter
        var loggerFactory = app.Services.GetService<ILoggerFactory>();
        var logger = loggerFactory?.CreateLogger("MigrateRemindersDatabase");

        var maxRetryAttempts = 5;
        var sleepBaseSeconds = 2;

        var policy = Policy.Handle<Exception>()
            .WaitAndRetry(maxRetryAttempts, retryAttempt =>
            {
                // exponential backoff with jitter
                var jitter = TimeSpan.FromMilliseconds(new Random().Next(0, 1000));
                return TimeSpan.FromSeconds(Math.Pow(sleepBaseSeconds, retryAttempt)) + jitter;
            }, (exception, timeSpan, retryCount, context) =>
            {
                logger?.LogWarning(exception, "Migration attempt {RetryCount} failed. Waiting {Delay} before next attempt.", retryCount, timeSpan);
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
                    throw new InvalidOperationException("RemindersContext service not available for migrations.");
                }

                logger?.LogInformation("Applying database migrations...");

                // Attempt to apply only provider-specific migrations when multiple
                // migrations for different providers are compiled into the same assembly.
                    try
                    {
                        // Prefer services registered in the current scope (useful for tests),
                        // otherwise fall back to the DbContext internal services.
                        var migrationsAssembly = scope?.ServiceProvider.GetService<IMigrationsAssembly>()
                                                 ?? db.GetInfrastructure().GetService<IMigrationsAssembly>();
                        var migrator = scope?.ServiceProvider.GetService<IMigrator>()
                                      ?? db.GetInfrastructure().GetService<IMigrator>();

                        if (migrationsAssembly is not null && migrator is not null)
                        {
                        foreach (var kv in migrationsAssembly.Migrations)
                        {
                            logger?.LogInformation("Discovered migration {MigrationId} in namespace {Namespace}", kv.Key, kv.Value?.Namespace);
                        }
                        var postgresMigrations = migrationsAssembly.Migrations
                            .Where(kv => (kv.Value?.Namespace ?? string.Empty).Contains(".Postgres."))
                            .Select(kv => kv.Key)
                            .OrderBy(id => id)
                            .ToList();

                        if (postgresMigrations.Any())
                        {
                            var target = postgresMigrations.Last();
                            logger?.LogInformation("Applying Postgres migrations up to {Migration}", target);
                            migrator.Migrate(target);
                        }
                        else
                        {
                            db.Database.Migrate();
                        }
                    }
                    else
                    {
                        db.Database.Migrate();
                    }
                }
                catch
                {
                    // fallback to default migrate if anything unexpected happens
                    db.Database.Migrate();
                }

                logger?.LogInformation("Database migrations applied successfully.");
            });
        }
        catch (Exception ex)
        {
            // If migrations still fail after retries, log as error and continue.
            logger?.LogError(ex, "Failed to apply database migrations after retries.");
        }

        return app;
    }
}