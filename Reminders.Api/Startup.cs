using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Business.RepositoryEntities;
using Reminders.Context.RemindersContext;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;
using AutoMapper;

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
            services.AddScoped<IRepositoryEntityGeneric<ReminderEntity>, RepositoryReminderEntity>();

            services.AddScoped<IBusinessModelGeneric<ReminderModel>, BusinessReminderModel>();

            services.AddDbContext<RemindersDbContext>(options =>
                      options.UseSqlServer(Configuration.GetConnectionString("StringConnectionReminders")));

            services.AddAutoMapper();

            services.AddLogging();

            services.AddMvc();
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
            }
            app.UseDeveloperExceptionPage();

            app.UseMvc();

        }
    }
}
