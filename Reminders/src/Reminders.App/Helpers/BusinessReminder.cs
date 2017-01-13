using Reminders.App.HelpersContract;
using Reminders.App.Models;
using Reminders.Data.Entity;
using Reminders.Domain.Contract;
using Reminders.Domain.Repository;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Reminders.App.Helpers
{
    public class BusinessReminder : IBusinessReminder
    {
        private IRepository<ReminderEntity> _repository;
        public BusinessReminder(IRepository<ReminderEntity> repository)
        {
            _repository = repository;
        }
        public void Insert(ReminderViewModel reminderViewModel)
        {

            var reminder = new ReminderEntity
            {
                Title = reminderViewModel.Title,
                Description = reminderViewModel.Description,
                LimitDate = reminderViewModel.LimitDate,
                IsDone = reminderViewModel.IsDone
            };

            _repository.Insert(reminder);
        }

        public void Update(ReminderViewModel reminderViewModel)
        {
            var reminder = new ReminderEntity
            {
                ID = reminderViewModel.ID,
                Title = reminderViewModel.Title,
                Description = reminderViewModel.Description,
                LimitDate = reminderViewModel.LimitDate,
                IsDone = reminderViewModel.IsDone
            };

            _repository.Update(reminder);
        }

        public void Delete(int key)
        {
            var reminder = _repository.Find(key);
            _repository.Delete(reminder);
        }

        public ReminderViewModel Find(int key)
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

        public List<ReminderViewModel> GetAll()
        {
            var reminders = _repository.GetAll().ToList();
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

    }
}
