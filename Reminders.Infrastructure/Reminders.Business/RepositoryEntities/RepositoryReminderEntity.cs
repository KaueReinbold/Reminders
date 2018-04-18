using Microsoft.EntityFrameworkCore;
using Reminders.Business.Contracts;
using Reminders.Context.RemindersContext;
using Reminders.Domain.Entities;
using System;
using System.Collections.Generic;
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

        public void Delete(ReminderEntity entity)
        {
            using (_remindersDbContext)
            {
                _remindersDbContext.Reminders.Remove(entity);

                _remindersDbContext.SaveChanges();
            }
        }

        public ReminderEntity Find(int key)
        {
            return _remindersDbContext.Reminders.Find(key);
        }

        public IEnumerable<ReminderEntity> GetAll()
        {
            var reminders = _remindersDbContext.Reminders;

            return reminders;
        }

        public void Insert(ReminderEntity entity)
        {
            using (_remindersDbContext)
            {
                _remindersDbContext.Reminders.Add(entity);

                _remindersDbContext.SaveChanges();
            }
        }

        public void Update(ReminderEntity entity)
        {
            using (_remindersDbContext)
            {
                _remindersDbContext.Attach(entity).State = EntityState.Modified;

                _remindersDbContext.SaveChanges();
            }
        }
    }
}
