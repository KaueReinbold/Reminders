using Reminders.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reminders.App.HelpersContract
{
    interface IHelperReminder
    {
        void Insert(ReminderViewModel reminderViewModel);
        void Update(ReminderViewModel reminderViewModel);
        void Delete(int key);
        List<ReminderViewModel> GetAll();
        ReminderViewModel Find(int key);
    }
}
