using System;
using System.Net;
using System.Runtime.Serialization;

namespace Reminders.Application.Exceptions
{
    public class RemindersApplicationException
        : Exception
    {
        public HttpStatusCode HttpStatusCode { get; }

        public RemindersApplicationException(HttpStatusCode httpStatusCode, string message) 
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
