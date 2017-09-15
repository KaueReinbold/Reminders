using Reminders.Domain.BusinessContract;
using Reminders.Data.Entity;
using Reminders.Domain.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using Reminders.Domain.Models;

namespace Reminders.Domain.Business
{
    /// <summary>
    /// Class of interaction between ViewModel and Entity.
    /// </summary>
    public class BusinessReminder : IBusinessReminder
    {
        private IRepository<ReminderEntity> _repository;

        public BusinessReminder(IRepository<ReminderEntity> repository)
        {
            _repository = repository;
        }

        public bool Insert(ReminderModel ReminderModel)
        {
            try
            {
                var reminder = new ReminderEntity
                {
                    title = ReminderModel.title,
                    description = ReminderModel.description,
                    limit_date = ReminderModel.limit_date.Value,
                    is_done = ReminderModel.is_done
                };

                _repository.Insert(reminder);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(ReminderModel ReminderModel)
        {
            try
            {
                var reminder = _repository.Find(ReminderModel.id);

                reminder.title = ReminderModel.title;
                reminder.description = ReminderModel.description;
                reminder.limit_date = ReminderModel.limit_date.Value;
                reminder.is_done = ReminderModel.is_done;

                _repository.Update(reminder);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(int key)
        {
            try
            {
                var reminder = _repository.Find(key);

                _repository.Delete(reminder);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public ReminderModel Find(int key)
        {
            try
            {
                var reminder = _repository.Find(key);

                var ReminderModel = new ReminderModel
                {
                    id = reminder.id,
                    title = reminder.title,
                    description = reminder.description,
                    limit_date = reminder.limit_date,
                    is_done = reminder.is_done
                };

                return ReminderModel;

            }
            catch (Exception)
            {
                return null;
            }

        }

        public List<ReminderModel> GetAll()
        {
            try
            {
                var reminders = _repository.GetAll().OrderBy(r => r.title).ToList();
                var remindersViewModel = new List<ReminderModel>();

                reminders.ForEach(r =>
                {
                    var reminder = new ReminderModel
                    {
                        id = r.id,
                        title = r.title,
                        description = r.description,
                        limit_date = r.limit_date,
                        is_done = r.is_done
                    };
                    remindersViewModel.Add(reminder);
                });

                return remindersViewModel;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
