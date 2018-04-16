using Reminders.Business.Contracts;
using Reminders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Business.BusinessModels
{
    public class BusinessReminderModel : IBusinessModelGeneric<ReminderModel>
    {
        public bool Delete(int key)
        {
            throw new NotImplementedException();
        }

        public ReminderModel Find(int key)
        {
            throw new NotImplementedException();
        }

        public List<ReminderModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Insert(ReminderModel model)
        {
            throw new NotImplementedException();
        }

        public bool Update(ReminderModel model)
        {
            throw new NotImplementedException();
        }
    }
}
