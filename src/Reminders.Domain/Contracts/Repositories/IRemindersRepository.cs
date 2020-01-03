using Reminders.Domain.Models;
using System;

namespace Reminders.Domain.Contracts.Repositories
{
    public interface IRemindersRepository
        : IRepository<Reminder>
    {
        bool Exists(Guid id);
    }
}
