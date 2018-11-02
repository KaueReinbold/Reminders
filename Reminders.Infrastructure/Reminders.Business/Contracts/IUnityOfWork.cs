using Reminders.Business.Contracts.Entity;
using Reminders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Business.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IReminderRepository RemindersRepository { get; }
        int Complete();
        void RejectChanges();
    }
}
