using Microsoft.AspNetCore.Mvc;
using Reminders.Application.Validators.Reminders.Exceptions;
using System.Net;
using ValidationException = FluentValidation.ValidationException;

namespace Reminders.Api.Extensions
{
    // One error shape for the whole API: RFC 7807 problem details, matching the Go
    // and C++ implementations behind the same load balancer (ADR-0011).
    public static class RemindersProblemDetailsFactory
    {
        public const string ClientErrorType = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
        public const string ServerErrorType = "https://tools.ietf.org/html/rfc9110#section-15.6.1";

        public static ProblemDetails Create(Exception exception) =>
            exception switch
            {
                ValidationException validationException => new ValidationProblemDetails(
                    validationException.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(error => error.ErrorMessage).ToArray()))
                {
                    Type = ClientErrorType,
                    Title = "One or more validation errors occurred.",
                    Status = (int)HttpStatusCode.BadRequest
                },

                RemindersApplicationException applicationException => new ProblemDetails
                {
                    Type = ClientErrorType,
                    Title = applicationException.Message,
                    Status = (int)applicationException.ToHttpStatusCode()
                },

                _ => new ProblemDetails
                {
                    Type = ServerErrorType,
                    Title = "Internal Server Error.",
                    Status = (int)HttpStatusCode.InternalServerError
                }
            };
    }
}
