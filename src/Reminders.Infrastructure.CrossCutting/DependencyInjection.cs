using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Infrastructure.Data.EntityFramework;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;
using Reminders.Infrastructure.Data.EntityFramework.Repositories;

namespace Reminders.Infrastructure.CrossCutting
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterDataServices(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services
                .AddDbContext<RemindersContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .AddScoped<IRemindersRepository, RemindersRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork<RemindersContext>>()
            ;

        public static IApplicationBuilder MigrateDatabase(
            this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

            scope.ServiceProvider.GetService<RemindersContext>().Database.Migrate();

            return app;
        }
    }
}