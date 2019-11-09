using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Application.Contracts;
using Reminders.Application.Mapper.Extensions;
using Reminders.Application.Services;
using Reminders.Infrastructure.CrossCutting.IoC;

namespace Reminders.Application.Extensions
{
    public static class ApplicationConfiguration
    {
        public static IServiceCollection RegisterApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services
                .AddSingleton(AutoMapperConfiguration.CreateMapper())
                .RegisterDataServices(configuration)
                .AddScoped<IRemindersService, RemindersService>()
            ;

    }
}
