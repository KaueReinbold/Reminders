using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Reminders.Api.Extensions;
using Reminders.Application.Extensions;
using Reminders.Infrastructure.CrossCutting.IoC;

namespace Reminders.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services
                .RegisterApplicationServices(Configuration)
                .AddControllers();

            services
                .AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reminders API", Version = "v1" }));
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logger)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app
                .UseHandleExceptionMiddleware()
                //.ConfigureExceptionHandler()
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers())
                .MigrateDatabase()
                .UseSwagger()
                .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reminders API V1"));
        }
    }
}
