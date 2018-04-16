using Reminders.Business.Contracts;
using Reminders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Business.RepositoryEntities
{
    public class RepositoryReminderEntity : IRepositoryEntityGeneric<ReminderEntity>
    {
        public void Delete(ReminderEntity entity)
        {
            throw new NotImplementedException();
        }

        public ReminderEntity Find(int key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReminderEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Insert(ReminderEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ReminderEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
