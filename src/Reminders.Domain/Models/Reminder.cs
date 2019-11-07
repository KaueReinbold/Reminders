using Reminders.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Domain.Models
{
    public class Reminder
        : Entity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LimitDate { get; set; }
        public bool IsDone { get; set; }
    }
}
