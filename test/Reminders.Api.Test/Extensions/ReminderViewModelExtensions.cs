using Reminders.Application.ViewModels;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Reminders.Api.Test.Extensions
{
    public static class ReminderViewModelExtensions
    {
        public static StringContent ToStringContent(this ReminderViewModel reminder)
            => new StringContent(JsonSerializer.Serialize(reminder), Encoding.UTF8, "application/json");
    }
}
