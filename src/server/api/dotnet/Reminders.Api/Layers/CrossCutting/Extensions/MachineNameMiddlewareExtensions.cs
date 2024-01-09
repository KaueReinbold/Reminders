namespace Reminders.Infrastructure.CrossCutting.Extensions;

public static class MachineNameMiddlewareExtensions
{
    public static IApplicationBuilder UseMachineNameLogging<T>(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            context
                .RequestServices
                .GetRequiredService<ILogger<T>>()
                .LogInformation($"\t\nMachineName: {Environment.MachineName} \t\nSystem: {Environment.OSVersion.VersionString} \t\nDateTime: {DateTime.UtcNow} \n");

            await next.Invoke();
        });

        return app;
    }
}
