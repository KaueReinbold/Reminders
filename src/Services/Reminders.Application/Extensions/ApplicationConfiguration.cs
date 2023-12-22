using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Application.Contracts;
using Reminders.Application.Enumerables;
using Reminders.Application.Mapper.Extensions;
using Reminders.Application.Services;
using Reminders.Application.Validators.Reminders;
using Reminders.Application.ViewModels;
using Reminders.Infrastructure.CrossCutting;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;

namespace Reminders.Application.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationConfiguration
    {
        public static IServiceCollection RegisterApplicationServices(
            this IServiceCollection services,
            string connectionString,
            SupportedDatabases supportedDatabases = SupportedDatabases.SqlServer)
        {
            services
                .AddSingleton(AutoMapperConfiguration.CreateMapper());

            if (supportedDatabases == SupportedDatabases.Sqlite)
                services.RegisterDataServicesSqlite(connectionString);
            else
                services.RegisterDataServices(connectionString);

            services.AddScoped<IRemindersService, RemindersService>();

            return services;
        }

        public static IMvcBuilder AddApplicationValidations(
            this IMvcBuilder mvcBuilder,
            IServiceCollection services)
        {
            mvcBuilder.AddFluentValidation();

            services.AddTransient<IValidator<ReminderViewModel>, ReminderViewModelValidator>();

            return mvcBuilder;
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
}