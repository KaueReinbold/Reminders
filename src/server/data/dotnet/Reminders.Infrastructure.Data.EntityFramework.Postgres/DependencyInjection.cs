using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;

namespace Reminders.Infrastructure.Data.EntityFramework.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection RegisterNpgsql(
        this IServiceCollection services,
        string connectionString)
    {
        services
            .AddDbContext<RemindersContext>(options =>
            {
                options.UseNpgsql(
                    connectionString,
                    _ => _.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName));
            })
            .RegisterDatabaseDependencies();

        return services;
    }
}
