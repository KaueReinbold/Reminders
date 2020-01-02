using Reminders.Application.Validators.Reminders.Exceptions.Enumerables;
using System;

namespace Reminders.Application.Validators.Reminders.Exceptions
{
    public class RemindersApplicationException
        : Exception
    {
        public StatusCode StatusCode { get; }

        public RemindersApplicationException(StatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
