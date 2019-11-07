using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;

namespace Reminders.Infrastructure.Data.EntityFramework.Repositories
{
    public class ReminderRepository
        : Repository<Reminder>, IRemindersRepository
    {
        public ReminderRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
