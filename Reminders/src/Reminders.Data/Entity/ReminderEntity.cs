using System;

namespace Reminders.Data.Entity
{
    public class ReminderEntity
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LimitDate { get; set; }
        public bool IsDone { get; set; }
    }
}
