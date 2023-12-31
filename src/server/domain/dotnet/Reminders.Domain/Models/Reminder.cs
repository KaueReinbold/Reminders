using System;

namespace Reminders.Domain.Models
{
    public class Reminder
        : Entity<Guid>
    {
        public Reminder() { }
        public Reminder(
            string title,
            string description,
            DateTime limitDate,
            bool isDone)
         : base(Guid.NewGuid(), false)
        {
            Title = title;
            Description = description;
            LimitDate = limitDate;
            IsDone = isDone;
        }

        public string Title { get; protected set; }
        public string Description { get; protected set; }
        public DateTime LimitDate { get; protected set; }
        public bool IsDone { get; protected set; }
    }
}