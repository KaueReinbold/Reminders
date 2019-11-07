using Microsoft.Extensions.DependencyInjection;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Infrastructure.Data.EntityFramework;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;
using Reminders.Infrastructure.Data.EntityFramework.Repositories;

namespace Reminders.Infrastructure.CrossCutting.IoC
{
    public static class DependencyInjectionBootstrapper
    {
        public static IServiceCollection RegisterDataServices(
            this IServiceCollection services) =>
            services
                .AddScoped<IRemindersRepository, ReminderRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork<RemindersContext>>()
            ;
    }
}
