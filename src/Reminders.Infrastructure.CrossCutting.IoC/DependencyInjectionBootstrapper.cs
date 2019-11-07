using Microsoft.Extensions.DependencyInjection;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Infrastructure.Data.Context;
using Reminders.Infrastructure.Data.EntityFramework;
using Reminders.Infrastructure.Data.EntityFramework.Repositories;

namespace Reminders.Infrastructure.CrossCutting.IoC
{
    public static class DependencyInjectionBootstrapper
    {
        public static IServiceCollection RegisterDateServices(
            this IServiceCollection services) =>
            services
                .AddScoped<IReminderRepository, ReminderRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork<RemindersContext>>()
            ;
    }
}
