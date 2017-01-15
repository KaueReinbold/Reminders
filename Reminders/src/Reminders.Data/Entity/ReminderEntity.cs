using System;

namespace Reminders.Data.Entity
{
    /// <summary>
    /// Class that represents the Reminder entity.
    /// </summary>
    public class ReminderEntity
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LimitDate { get; set; }
        public bool IsDone { get; set; }
    }
}
