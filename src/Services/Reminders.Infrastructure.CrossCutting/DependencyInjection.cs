using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Infrastructure.Data.EntityFramework;
using Reminders.Infrastructure.Data.EntityFramework.Enumerables;

namespace Reminders.Infrastructure.CrossCutting
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationConfiguration
    {
        public static IServiceCollection RegisterDataServices(
                this IServiceCollection services,
                string connectionString) =>
            services
                .RegisterEntityFrameworkServices(connectionString);

        public static IServiceCollection RegisterDataServicesSqlite(
                this IServiceCollection services,
                string databasePath) =>
            services
                .RegisterEntityFrameworkServices(databasePath, SupportedDatabases.Sqlite);
    }
}