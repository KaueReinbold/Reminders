using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Infrastructure.Data.Dapper.Repositories;

namespace Reminders.Infrastructure.Data.Dapper
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterDapperServices(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services
                .AddScoped<IRemindersRepository, RemindersRepository>();
    }
}
