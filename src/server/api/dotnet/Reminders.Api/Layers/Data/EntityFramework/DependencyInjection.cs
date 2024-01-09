namespace Reminders.Infrastructure.Data.EntityFramework;

public static class DependencyInjection
{
    public static IServiceCollection RegisterDatabaseDependencies(
        this IServiceCollection services)
    {
        services
            .AddScoped<IRemindersRepository, RemindersRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork<RemindersContext>>();

        return services;
    }

    public static IApplicationBuilder MigrateDatabase<TContext>(
        this IApplicationBuilder app)
        where TContext : DbContext
    {
        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

        scope.ServiceProvider.GetService<TContext>()?.Database.Migrate();

        return app;
    }
}
