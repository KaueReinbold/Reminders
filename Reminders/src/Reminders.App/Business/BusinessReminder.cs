using Reminders.App.BusinessContract;
using Reminders.App.Models;
using Reminders.Data.Entity;
using Reminders.Domain.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Reminders.App.Business
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

        public bool Insert(ReminderViewModel reminderViewModel)
        {
            try
            {
                var reminder = new ReminderEntity
                {
                    Title = reminderViewModel.Title,
                    Description = reminderViewModel.Description,
                    LimitDate = reminderViewModel.LimitDate.Value,
                    IsDone = reminderViewModel.IsDone
                };

                _repository.Insert(reminder);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ReminderViewModel reminderViewModel)
        {
            try
            {
                var reminder = _repository.Find(reminderViewModel.ID);

                reminder.Title = reminderViewModel.Title;
                reminder.Description = reminderViewModel.Description;
                reminder.LimitDate = reminderViewModel.LimitDate.Value;
                reminder.IsDone = reminderViewModel.IsDone;

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

        public ReminderViewModel Find(int key)
        {
            try
            {
                var reminder = _repository.Find(key);

                var reminderViewModel = new ReminderViewModel
                {
                    ID = reminder.ID,
                    Title = reminder.Title,
                    Description = reminder.Description,
                    LimitDate = reminder.LimitDate,
                    IsDone = reminder.IsDone
                };

                return reminderViewModel;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ReminderViewModel> GetAll()
        {
            try
            {
                var reminders = _repository.GetAll().OrderBy(r => r.Title).ToList();
                var remindersViewModel = new List<ReminderViewModel>();

                reminders.ForEach(r =>
                {
                    var reminder = new ReminderViewModel
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
