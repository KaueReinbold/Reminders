using System;

namespace Reminders.Data.Entity
{
    /// <summary>
    /// Class that represents the Reminder entity.
    /// </summary>
    public class ReminderEntity
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime limit_date { get; set; }
        public bool is_done { get; set; }
    }
}
