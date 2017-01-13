using Reminders.App.Models;
using System.Collections.Generic;

namespace Reminders.App.BusinessContract
{
    interface IBusinessReminder
    {
        void Insert(ReminderViewModel reminderViewModel);
        void Update(ReminderViewModel reminderViewModel);
        void Delete(int key);
        List<ReminderViewModel> GetAll();
        ReminderViewModel Find(int key);
    }
}
