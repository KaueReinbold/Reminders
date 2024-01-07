namespace Reminders.Api.Extensions;

public static class CorsExtensions
{
    public const string RemindersCorsPolicyName = "AllowSpecificOrigin";

    public static IServiceCollection AddRemindersCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(RemindersCorsPolicyName,
                policyBuilder =>
                {
                    var origins = configuration["CorsOrigins"]?.Split(",");
                    if (origins != null)
                    {
                        policyBuilder.WithOrigins(origins)
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                    }
                });
        });

        return services;
    }

    public static IApplicationBuilder UseRemindersCors(this IApplicationBuilder app)
    {
        app.UseCors(RemindersCorsPolicyName);

        return app;
    }
}