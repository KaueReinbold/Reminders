using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Infrastructure.Data.EntityFramework.Postgres;
using Reminders.Infrastructure.Data.EntityFramework.SqlServer;

namespace Reminders.Infrastructure.CrossCutting
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationConfiguration
    {
        public static IServiceCollection RegisterDataServicesSqlServer(
                this IServiceCollection services,
                string connectionString) =>
            services
                .RegisterSqlServer(connectionString);

        public static IServiceCollection RegisterDataServicesPostgres(
                this IServiceCollection services,
                string connectionString) =>
            services
                .RegisterNpgsql(connectionString);
    }
}