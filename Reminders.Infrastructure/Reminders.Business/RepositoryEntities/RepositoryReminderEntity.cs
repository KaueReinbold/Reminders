using Microsoft.EntityFrameworkCore;
using Reminders.Business.Contracts;
using Reminders.Context.RemindersContext;
using Reminders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reminders.Business.RepositoryEntities
{
    public class RepositoryReminderEntity : IRepositoryEntityGeneric<ReminderEntity>
    {
        private readonly RemindersDbContext _remindersDbContext;

        public RepositoryReminderEntity(RemindersDbContext remindersDbContext)
        {
            _remindersDbContext = remindersDbContext;
        }

        public ReminderEntity Find(int key)
        {
            return _remindersDbContext.Reminders.Find(key);
        }

        public IEnumerable<ReminderEntity> GetAll()
        {
            var reminders = _remindersDbContext.Reminders.AsNoTracking().ToList();

            return reminders;
        }

        public ReminderEntity Insert(ReminderEntity entity)
        {
            _remindersDbContext.Reminders.Add(entity);

            _remindersDbContext.SaveChanges();

            return entity;
        }

        public bool Update(ReminderEntity entity)
        {
            int result = 0;

            _remindersDbContext.Attach(entity).State = EntityState.Modified;

            result = _remindersDbContext.SaveChanges();

            return result > 0;
        }

        public bool Delete(ReminderEntity entity)
        {
            int result = 0;

            _remindersDbContext.Reminders.Remove(entity);

            result = _remindersDbContext.SaveChanges();

            return result > 0;
        }
    }
}
