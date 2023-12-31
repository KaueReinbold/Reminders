using Reminders.Domain.Models;

namespace Reminders.Domain.Contracts.Repositories
{
    public interface IRemindersRepository
        : IRepository<Reminder>
    { }
}
