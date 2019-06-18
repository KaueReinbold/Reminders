using Reminders.Domain.Enums;

namespace Reminders.Domain.Models
{
    public class MessageModel
    {
        public EnumMessages Type { get; set; }
        public string Message { get; set; }
    }
}
