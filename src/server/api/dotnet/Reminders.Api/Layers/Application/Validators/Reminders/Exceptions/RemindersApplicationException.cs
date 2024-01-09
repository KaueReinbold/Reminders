namespace Reminders.Application.Validators.Reminders.Exceptions;

public class RemindersApplicationException
    : Exception
{
    public ValidationStatus StatusCode { get; }

    public RemindersApplicationException(
        ValidationStatus statusCode,
        string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
