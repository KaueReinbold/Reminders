using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Reminders.Api.Models;
using Reminders.Application.Validators.Reminders.Exceptions;
using System.Net;

namespace Reminders.Api.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder ConfigureExceptionHandler(
            this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
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
                            statusCode = (int)remindersApplicationException.ToHttpStatusCode();
                            message = remindersApplicationException.Message;
                            context.Response.StatusCode = statusCode;
                        }

                        //logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = statusCode,
                            Message = message
                        }.ToString());
                    }
                });
            });

            return app;
        }
    }
}
