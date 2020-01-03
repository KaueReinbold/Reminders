using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;
using System;

namespace Reminders.Infrastructure.Data.EntityFramework.Repositories
{
    public class RemindersRepository
        : Repository<Reminder>, IRemindersRepository
    {
        public RemindersRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public bool Exists(Guid id)
        {
            var reminder = GetAsNoTracking(id);

            return !(reminder is null) && !reminder.IsDeleted;
        }
    }
}
