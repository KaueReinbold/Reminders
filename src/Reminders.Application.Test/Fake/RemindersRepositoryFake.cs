using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reminders.Application.Test.Fake
{
    public class RemindersRepositoryFake
        : IRemindersRepository
    {
        public List<Reminder> Reminders = new List<Reminder>();

        public void Add(Reminder reminder) => Reminders.Add(reminder);

        public void Dispose() { }

        public bool Exists(Guid id)
        {
            var reminder = Reminders.Find(r => r.Id == id);

            return !(reminder is null) && !reminder.IsDeleted;
        }

        public Reminder Get(Guid id) => Reminders.Find(r => r.Id == id);

        public IQueryable<Reminder> Get() => Reminders.AsQueryable();

        public Reminder GetAsNoTracking(Guid id) => Reminders.Find(r => r.Id == id);

        public IQueryable<Reminder> GetAsNoTracking() => Reminders.AsQueryable();

        public void Remove(Guid id) => Reminders.Remove(Reminders.Find(r => r.Id == id));

        public int SaveChanges() => 1;

        public void Update(Reminder reminder)
        {
            Reminders.RemoveAll(r => r.Id == reminder.Id);
            Reminders.Add(reminder);
        }
    }
}
