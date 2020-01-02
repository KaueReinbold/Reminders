﻿using Reminders.Application.Validators.Reminders.Exceptions;
using Reminders.Application.Validators.Reminders.Exceptions.Enumerables;
using System.Net;

namespace Reminders.Api.Extensions
{
    public static class RemindersApplicationExceptionExtensions
    {
        public static HttpStatusCode ToHttpStatusCode(this RemindersApplicationException remindersApplicationException) =>
            remindersApplicationException.StatusCode switch
            {
                StatusCode.NotFound => HttpStatusCode.NotFound,
                StatusCode.IdsDoNotMatch => HttpStatusCode.Conflict,
                _ => HttpStatusCode.BadRequest,
            };
    }
}
