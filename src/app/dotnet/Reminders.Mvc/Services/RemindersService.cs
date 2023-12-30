namespace Reminders.Mvc.Services;

public class ApiException : Exception
{
    public ErrorResponse? ErrorResponse { get; }

    public ApiException(string message, ErrorResponse? errorResponse) : base(message)
    {
        ErrorResponse = errorResponse;
    }
}

public class RemindersService : IRemindersService
{
    private readonly HttpClient httpClient;
    private readonly string remoteServiceBaseUrl = "/api/reminders";

    public RemindersService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<ReminderViewModel>?> GetRemindersAsync()
    {
        var response = await httpClient.GetAsync(remoteServiceBaseUrl);

        response.EnsureSuccessStatusCode();

        var content = response.Content.ReadAsStringAsync().Result;
        var reminders = JsonConvert.DeserializeObject<IEnumerable<ReminderViewModel>?>(content);

        return reminders;
    }

    public async Task<ReminderViewModel?> GetReminderAsync(Guid id)
    {
        var response = await httpClient.GetAsync($"{remoteServiceBaseUrl}/{id}");

        response.EnsureSuccessStatusCode();

        var content = response.Content.ReadAsStringAsync().Result;
        var reminder = JsonConvert.DeserializeObject<ReminderViewModel>(content);

        return reminder;
    }

    public async Task AddReminderAsync(ReminderViewModel reminderViewModel)
    {
        using StringContent jsonContent = new(
            JsonConvert.SerializeObject(reminderViewModel),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PostAsync(remoteServiceBaseUrl, jsonContent);

        ValidateApiResponse(response);

        response.EnsureSuccessStatusCode();
    }

    public async Task EditReminderAsync(Guid id, ReminderViewModel reminderViewModel)
    {
        using StringContent jsonContent = new(
            JsonConvert.SerializeObject(reminderViewModel),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PutAsync($"{remoteServiceBaseUrl}/{id}", jsonContent);

        ValidateApiResponse(response);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteReminderAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync($"{remoteServiceBaseUrl}/{id}");

        response.EnsureSuccessStatusCode();
    }

    private void ValidateApiResponse(HttpResponseMessage? response)
    {
        if (response?.IsSuccessStatusCode == false)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);

            throw new ApiException("API returned an error.", errorResponse);
        }
    }
}
