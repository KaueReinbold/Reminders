namespace Reminders.Mvc.Extensions.Handlers;

public class RemindersServiceHealthCheck : IHealthCheck
{
    private readonly ApiOptions apiOptions;
    private readonly IHttpClientFactory httpClientFactory;

    public RemindersServiceHealthCheck(
        IOptions<ApiOptions> apiOptions,
        IHttpClientFactory httpClientFactory)
    {
        this.apiOptions = apiOptions.Value;
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(nameof(RemindersService));
            var response = await httpClient.GetAsync(apiOptions.HealthUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Reminders service is healthy.");
            }
            else
            {
                var errorMessage = $"Reminders service failed with status code: {response.StatusCode}.";

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    errorMessage = "The health check endpoint was not found.";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    errorMessage = "The service is currently unavailable.";
                }

                return HealthCheckResult.Unhealthy(errorMessage);
            }
        }
        catch (HttpRequestException ex)
        {
            var errorMessage = $"Reminders service failed with an HTTP request exception: {ex.Message}";

            if (ex.InnerException is System.Net.Sockets.SocketException socketException)
            {
                errorMessage = "Unable to connect to the health check endpoint. Check the network connection.";
            }

            return HealthCheckResult.Unhealthy(errorMessage);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Reminders service failed with an exception: {ex.Message}");
        }
    }
}
