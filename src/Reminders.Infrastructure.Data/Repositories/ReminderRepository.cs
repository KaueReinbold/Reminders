using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;

namespace Reminders.Infrastructure.Data.EntityFramework.Repositories
{
    public class RemindersRepository
        : Repository<Reminder>, IRemindersRepository
    {
        public RemindersRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
