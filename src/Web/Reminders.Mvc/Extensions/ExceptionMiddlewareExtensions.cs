using Microsoft.AspNetCore.Diagnostics;
using Reminders.Application.Validators.Reminders.Exceptions;
using System.Net;

namespace Reminders.Mvc.Extensions
{
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
                        var statusCode = context.Response.StatusCode;
                        var message = "Internal Server Error.";

                        if (contextFeature.Error is RemindersApplicationException remindersApplicationException)
                        {
                            message = remindersApplicationException.Message;
                            context.Response.StatusCode = statusCode;
                        }
                        else
                            context.Response.Redirect("/Home/Error");
                    }

                    return Task.CompletedTask;
                });
            });

            return app;
        }
    }
}
