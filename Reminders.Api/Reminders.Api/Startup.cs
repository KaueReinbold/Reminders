using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Business.RepositoryEntities;
using Reminders.Context.RemindersContext;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;
using AutoMapper;
using Microsoft.Azure.KeyVault.Models;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

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
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "Reminders",
                        Version = "v1",
                        Description = "Application to register and list reminders. Each reminder has a Title, Description, Date Limit and Status.",
                        Contact = new Swashbuckle.AspNetCore.Swagger.Contact
                        {
                            Name = "Kaue Reinbold",
                            Url = "https://github.com/KaueReinbold"
                        }
                    });

                string caminhoAplicacao =
                    PlatformServices.Default.Application.ApplicationBasePath;
                string nomeAplicacao =
                    PlatformServices.Default.Application.ApplicationName;
                string caminhoXmlDoc =
                    Path.Combine(caminhoAplicacao, $"{nomeAplicacao}.xml");

                c.IncludeXmlComments(caminhoXmlDoc);
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

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "Reminders");
            });
        }
    }
}
