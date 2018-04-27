using Newtonsoft.Json;

namespace Reminders.Domain.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
