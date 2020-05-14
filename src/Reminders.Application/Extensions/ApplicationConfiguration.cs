using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Application.Contracts;
using Reminders.Application.Mapper.Extensions;
using Reminders.Application.Services;
using Reminders.Application.Validators.Reminders;
using Reminders.Application.ViewModels;
using Reminders.Infrastructure.CrossCutting;

namespace Reminders.Application.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationConfiguration
    {
        public static IServiceCollection RegisterApplicationServices(
                this IServiceCollection services,
                IConfiguration configuration) =>
            services
                .AddSingleton(AutoMapperConfiguration.CreateMapper())
                .RegisterDataServices(configuration)
                .AddScoped<IRemindersService, RemindersService>();

        public static IMvcBuilder AddApplicationValidations(
            this IMvcBuilder mvcBuilder,
            IServiceCollection services)
        {
            mvcBuilder.AddFluentValidation();

            services.AddTransient<IValidator<ReminderViewModel>, ReminderViewModelValidator>();

            return mvcBuilder;
        }
    }
}