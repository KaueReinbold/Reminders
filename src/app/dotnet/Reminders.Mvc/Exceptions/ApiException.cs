namespace Reminders.Mvc.Exceptions;

public class ApiException : Exception
{
    public const string TROUBLE_CONNECTING_SERVERS = "We're having trouble connecting to our servers. Please check your internet connection or contact the admin.";

    public ErrorResponse? ErrorResponse { get; }

    public ApiException(string message, ErrorResponse? errorResponse = null) : base(message)
    {
        ErrorResponse = errorResponse;
    }
}