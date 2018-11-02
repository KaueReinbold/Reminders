using Reminders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Business.Contracts.Entity
{
    public interface IReminderRepository 
        : IEntityRepositoryGeneric<ReminderEntity>
    {
    }
}
