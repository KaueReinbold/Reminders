using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Reminders.Api.Extensions;
using Reminders.Application.Enumerables;
using Reminders.Application.Extensions;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;

namespace Reminders.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .RegisterApplicationServices(
                    Configuration.GetConnectionString("DefaultConnection"),
                    SupportedDatabases.SqlServer)
                .AddControllers()
                .AddApplicationValidations(services);

            services
                .AddSwaggerGen(setup =>
                    setup.SwaggerDoc("v1", new OpenApiInfo { Title = "Reminders API", Version = "v1" }));
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logger, RemindersContext context)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app
                .UseRemindersExceptionHandler()
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers())
                .UseSwagger()
                .UseSwaggerUI(setup =>
                    setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Reminders API V1"));

            context.Database.Migrate();
        }
    }
}