using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Application.Contracts;
using Reminders.Application.Mapper.Extensions;
using Reminders.Application.Services;
using Reminders.Infrastructure.CrossCutting;

namespace Reminders.Application.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationConfiguration
    {
        public static IServiceCollection RegisterApplicationServices(
                this IServiceCollection services,
                IConfiguration configuration) =>
            services
                .AddSingleton(AutoMapperConfiguration.CreateMapper())
                .RegisterDataServices(configuration)
                .AddScoped<IRemindersService, RemindersService>();
    }
}