using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Domain.Models
{
    public class MessageModel
    {
        public EnumMessages Type { get; set; }
        public string Message { get; set; }
    }
}
