using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Reminders.Api.Models;
using Reminders.Application.Validators.Reminders.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Reminders.Api.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseRemindersExceptionHandler(
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
                        var statusCode = (int)HttpStatusCode.InternalServerError;
                        var message = "Internal Server Error.";
                        Dictionary<string, string> properties = null;

                        if (contextFeature.Error is RemindersApplicationException remindersApplicationException)
                        {
                            statusCode = (int)remindersApplicationException.ToHttpStatusCode();
                            message = remindersApplicationException.Message;
                        }
                        else if (contextFeature.Error is ValidationException validationException)
                        {
                            statusCode = (int)HttpStatusCode.UnprocessableEntity;
                            message = validationException.Message;
                            properties = validationException.Errors.ToDictionary(
                                error => error.PropertyName,
                                error => error.ErrorMessage);
                        }

                        context.Response.StatusCode = statusCode;

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = statusCode,
                            Message = message,
                            Properties = properties
                        }.ToString());
                    }
                });
            });

            return app;
        }
    }
}
