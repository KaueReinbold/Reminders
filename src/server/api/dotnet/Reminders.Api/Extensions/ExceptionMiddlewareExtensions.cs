using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerDefaults = System.Text.Json.JsonSerializerDefaults;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using JsonIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition;

namespace Reminders.Api.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public const string ProblemDetailsContentType = "application/problem+json";

        // Null members stay out of the body so the three implementations emit the
        // same problem details for the same failure (ADR-0011).
        private static readonly JsonSerializerOptions SerializerOptions =
            new(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        public static IApplicationBuilder UseRemindersExceptionHandler(
            this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature == null)
                        return;

                    var problemDetails = RemindersProblemDetailsFactory.Create(contextFeature.Error);

                    context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = ProblemDetailsContentType;

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(problemDetails, problemDetails.GetType(), SerializerOptions));
                });
            });

            return app;
        }
    }
}
