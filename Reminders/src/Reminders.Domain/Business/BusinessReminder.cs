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
                    Title = ReminderModel.Title,
                    Description = ReminderModel.Description,
                    LimitDate = ReminderModel.LimitDate.Value,
                    IsDone = ReminderModel.IsDone
                };

                _repository.Insert(reminder);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ReminderModel ReminderModel)
        {
            try
            {
                var reminder = _repository.Find(ReminderModel.ID);

                reminder.Title = ReminderModel.Title;
                reminder.Description = ReminderModel.Description;
                reminder.LimitDate = ReminderModel.LimitDate.Value;
                reminder.IsDone = ReminderModel.IsDone;

                _repository.Update(reminder);

                return true;
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
                    ID = reminder.ID,
                    Title = reminder.Title,
                    Description = reminder.Description,
                    LimitDate = reminder.LimitDate,
                    IsDone = reminder.IsDone
                };

                return ReminderModel;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ReminderModel> GetAll()
        {
            try
            {
                var reminders = _repository.GetAll().OrderBy(r => r.Title).ToList();
                var remindersViewModel = new List<ReminderModel>();

                reminders.ForEach(r =>
                {
                    var reminder = new ReminderModel
                    {
                        ID = r.ID,
                        Title = r.Title,
                        Description = r.Description,
                        LimitDate = r.LimitDate,
                        IsDone = r.IsDone
                    };
                    remindersViewModel.Add(reminder);
                });

                return remindersViewModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
