using Reminders.Application.ViewModels;
using System.Linq;

namespace Reminders.Application.Contracts
{
    public interface IRemindersService
    {
        void Insert(ReminderViewModel reminderViewModel);
        void Edit(int id, ReminderViewModel reminderViewModel);
        void Delete(int id);

        IQueryable<ReminderViewModel> Get();
        ReminderViewModel Get(int id);
    }
}
