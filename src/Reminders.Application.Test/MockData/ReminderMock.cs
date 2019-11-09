using Reminders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reminders.Application.Test.MockData
{
    public static class ReminderMock
    {
        public static IQueryable<Reminder> Reminders =>
            new List<Reminder>
            {
                new Reminder("Title 1", "Description 1", new DateTime(2019, 1, 1), false),
                new Reminder("Title 2", "Description 2", new DateTime(2019, 2, 2), false),
                new Reminder("Title 3", "Description 3", new DateTime(2019, 3, 3), false),
                new Reminder("Title 4", "Description 4", new DateTime(2019, 4, 5), false),
                new Reminder("Title 5", "Description 5", new DateTime(2019, 5, 5), false)
            }.AsQueryable();
    }
}
