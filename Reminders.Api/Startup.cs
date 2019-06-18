using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Context.RemindersContext;
using Reminders.Domain.Models;
using Swashbuckle.AspNetCore.Swagger;
using Reminders.Core.Options;
using Reminders.Business.Contracts.Business;
using Reminders.Business.RepositoryEntities.Persistence;
using Reminders.Domain.Extensions;

namespace Reminders.Api
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="configuration"> IConfiguration object. </param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// IConfiguration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"> IServiceCollection object. </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBusinessModelGeneric<ReminderModel>, BusinessReminderModel>();

            //services.AddDbContext<RemindersDbContext>(options =>
            //          options.UseSqlServer(Configuration.GetConnectionString("StringConnectionReminders")), ServiceLifetime.Scoped);

            services.AddDbContext<RemindersDbContext>(options =>
                      options.UseInMemoryDatabase("DbReminders"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddOptions();

            services.Configure<ApplicationOptions>(Configuration.GetSection("AppSettings"));

            // Add automapper
            services.AddAutoMapper();

            services.AddLogging();

            services.AddMvc();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Reminders Api",
                    Description = "Application to register and list reminders. Each Reminder a Title, Description, Date Limit and Status.",
                    Contact = new Contact
                    {
                        Name = "Kaue Reinbold",
                        Email = "ck_reinbold@hotmail.com",
                        Url = "https://github.com/KaueReinbold"
                    }
                });
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"> IApplicationBuilder object. </param>
        /// <param name="env"> IHostingEnvironment object. </param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reminders Api");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
