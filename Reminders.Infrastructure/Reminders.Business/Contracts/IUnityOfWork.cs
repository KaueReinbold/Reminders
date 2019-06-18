using Reminders.Business.Contracts.Entity;
using System;

namespace Reminders.Business.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IReminderRepository RemindersRepository { get; }
        int Complete();
        void RejectChanges();
    }
}
