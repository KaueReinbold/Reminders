namespace Reminders.Mvc.Options;

public partial class ApiOptions
{
    public const string ApiOptionsSectionName = "ApiOptions";

    public Uri? BaseUrl { get; set; }

    public string? HealthUrl { get; set; }
}