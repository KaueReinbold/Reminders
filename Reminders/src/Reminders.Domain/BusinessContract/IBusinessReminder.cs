using Reminders.Domain.Models;
using System.Collections.Generic;

namespace Reminders.Domain.BusinessContract
{
    /// <summary>
    /// Interaction contract between ViewModel and Entity.
    /// </summary>
    public interface IBusinessReminder
    {
        bool Insert(ReminderModel reminderViewModel);
        bool Update(ReminderModel reminderViewModel);
        bool Delete(int key);
        List<ReminderModel> GetAll();
        ReminderModel Find(int key);
    }
}
