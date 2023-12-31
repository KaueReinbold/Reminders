namespace Reminders.Mvc.Services;

public class RemindersService : IRemindersService
{
    private readonly HttpClient httpClient;
    private readonly HealthCheckService healthCheckService;
    private readonly string remoteServiceBaseUrl = "/api/reminders";

    public RemindersService(
        HttpClient httpClient,
        HealthCheckService healthCheckService)
    {
        this.httpClient = httpClient;
        this.healthCheckService = healthCheckService;
    }

    public async Task<IEnumerable<ReminderViewModel>?> GetRemindersAsync(CancellationToken cancellationToken)
    {
        await ValidateApiHealth(cancellationToken);

        var response = await httpClient.GetAsync(remoteServiceBaseUrl, cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var reminders = JsonConvert.DeserializeObject<IEnumerable<ReminderViewModel>?>(content);

        return reminders;
    }

    public async Task<ReminderViewModel?> GetReminderAsync(Guid id, CancellationToken cancellationToken)
    {
        await ValidateApiHealth(cancellationToken);

        var response = await httpClient.GetAsync($"{remoteServiceBaseUrl}/{id}", cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var reminder = JsonConvert.DeserializeObject<ReminderViewModel>(content);

        return reminder;
    }

    public async Task AddReminderAsync(ReminderViewModel reminderViewModel, CancellationToken cancellationToken)
    {
        await ValidateApiHealth(cancellationToken);

        using StringContent jsonContent = new(
            JsonConvert.SerializeObject(reminderViewModel),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PostAsync(remoteServiceBaseUrl, jsonContent, cancellationToken);

        await ValidateApiResponse(response, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task EditReminderAsync(Guid id, ReminderViewModel reminderViewModel, CancellationToken cancellationToken)
    {
        await ValidateApiHealth(cancellationToken);

        using StringContent jsonContent = new(
            JsonConvert.SerializeObject(reminderViewModel),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PutAsync($"{remoteServiceBaseUrl}/{id}", jsonContent, cancellationToken);

        await ValidateApiResponse(response, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteReminderAsync(Guid id, CancellationToken cancellationToken)
    {
        await ValidateApiHealth(cancellationToken);

        var response = await httpClient.DeleteAsync($"{remoteServiceBaseUrl}/{id}", cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private static async Task ValidateApiResponse(HttpResponseMessage? response, CancellationToken cancellationToken)
    {
        if (response?.IsSuccessStatusCode == false)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);

            throw new ApiException(ApiException.TROUBLE_CONNECTING_SERVERS, errorResponse);
        }
    }

    private async Task ValidateApiHealth(CancellationToken cancellationToken)
    {
        var healthReport = await healthCheckService.CheckHealthAsync(cancellationToken);

        if (healthReport.Status != HealthStatus.Healthy)
            throw new ApiException(ApiException.TROUBLE_CONNECTING_SERVERS);
    }
}
