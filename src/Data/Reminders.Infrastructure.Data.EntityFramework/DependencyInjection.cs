using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;
using Reminders.Infrastructure.Data.EntityFramework.Enumerables;
using Reminders.Infrastructure.Data.EntityFramework.Repositories;

namespace Reminders.Infrastructure.Data.EntityFramework
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterEntityFrameworkServices(
            this IServiceCollection services,
            string connectionString,
            SupportedDatabases supportedDatabases = SupportedDatabases.SqlServer) =>
            services
                .AddDbContext<RemindersContext>(options =>
                {
                    if (supportedDatabases == SupportedDatabases.Sqlite)
                        options.UseSqlite(connectionString);
                    else
                        options.UseSqlServer(connectionString);
                })
                .AddScoped<IRemindersRepository, RemindersRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork<RemindersContext>>()
            ;

        public static IApplicationBuilder MigrateDatabase<TContext>(
            this IApplicationBuilder app)
            where TContext : DbContext
        {
            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

            scope.ServiceProvider.GetService<TContext>().Database.Migrate();

            return app;
        }
    }
}