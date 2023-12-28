using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;

namespace Reminders.Infrastructure.Data.EntityFramework.SqlServer;

public static class DependencyInjection
{
    public static IServiceCollection RegisterSqlServer(
        this IServiceCollection services,
        string connectionString)
    {
        services
            .AddDbContext<RemindersContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    _ => _.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName));
            })
            .RegisterDatabaseDependencies();

        return services;
    }
}
