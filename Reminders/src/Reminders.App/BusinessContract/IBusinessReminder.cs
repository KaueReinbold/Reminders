using Reminders.App.Models;
using System.Collections.Generic;

namespace Reminders.App.BusinessContract
{
    /// <summary>
    /// Interaction contract between ViewModel and Entity.
    /// </summary>
    public interface IBusinessReminder
    {
        bool Insert(ReminderViewModel reminderViewModel);
        bool Update(ReminderViewModel reminderViewModel);
        bool Delete(int key);
        List<ReminderViewModel> GetAll();
        ReminderViewModel Find(int key);
    }
}
