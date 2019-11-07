using Reminders.Application.ViewModels;
using System.Linq;

namespace Reminders.Application.Contracts
{
    public interface IRemindersService
    {
        void Insert(ReminderViewModel reminderViewModel);
        void Edit(ReminderViewModel reminderViewModel);
        void Delete(ReminderViewModel reminderViewModel);

        IQueryable<ReminderViewModel> Get();
        ReminderViewModel Get(int id);
    }
}
