namespace Reminders.Mvc.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder ConfigureExceptionHandler(
        this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature != null)
                {
                    context.Response.Redirect("/Home/Error");
                }

                return Task.CompletedTask;
            });
        });

        return app;
    }
}