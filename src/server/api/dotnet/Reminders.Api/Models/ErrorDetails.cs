namespace Reminders.Api.Models;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string>? Properties { get; set; }

    public override string ToString() =>
        JsonConvert.SerializeObject(this);
}