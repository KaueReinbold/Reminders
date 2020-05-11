using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Infrastructure.Data.EntityFramework;

namespace Reminders.Infrastructure.CrossCutting
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationConfiguration
    {
        public static IServiceCollection RegisterDataServices(
                this IServiceCollection services,
                IConfiguration configuration) =>
            services
                .RegisterEntityFrameworkServices(configuration);
    }
}