namespace Reminders.Application.Extensions;

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
        try
        {
            var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory?.CreateScope();

            scope?.ServiceProvider.GetService<RemindersContext>()?.Database.Migrate();
        }
        catch // TODO: implement the better way to migrate the database.
        { }

        return app;
    }
}