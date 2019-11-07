using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;

namespace Reminders.Infrastructure.Data.EntityFramework.Repositories
{
    public class ReminderRepository
        : Repository<Reminder>, IReminderRepository
    {
        public ReminderRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
