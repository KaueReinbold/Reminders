using System.Text.Json;

namespace Reminders.Api.Test.Extensions
{
    public static class StringExtensions
    {
        public static T FromJson<T>(this string contentString)
        {
            return JsonSerializer.Deserialize<T>(contentString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
