using Reminders.App.Models;
using System.Collections.Generic;

namespace Reminders.App.BusinessContract
{
    public interface IBusinessReminder
    {
        bool Insert(ReminderViewModel reminderViewModel);
        bool Update(ReminderViewModel reminderViewModel);
        bool Delete(int key);
        List<ReminderViewModel> GetAll();
        ReminderViewModel Find(int key);
    }
}
