using Reminders.Application.Validators.Reminders.Exceptions;
using Reminders.Application.Validators.Reminders.Exceptions.Enumerables;
using System.Net;

namespace Reminders.Api.Extensions
{
    public static class RemindersApplicationExceptionExtensions
    {
        public static HttpStatusCode ToHttpStatusCode(this RemindersApplicationException remindersApplicationException) =>
            remindersApplicationException.StatusCode switch
            {
                ValidationStatus.NotFound => HttpStatusCode.NotFound,
                ValidationStatus.IdsDoNotMatch => HttpStatusCode.Conflict,
                _ => HttpStatusCode.BadRequest,
            };
    }
}
