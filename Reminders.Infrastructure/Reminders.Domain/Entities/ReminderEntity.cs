using System;

namespace Reminders.Domain.Entities
{
    /// <summary>
    /// Class that represents the Reminder entity.
    /// </summary>
    public class ReminderEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LimitDate { get; set; }
        public bool IsDone { get; set; }
    }
}
