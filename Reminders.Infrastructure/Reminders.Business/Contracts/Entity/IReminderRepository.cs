using Reminders.Domain.Entities;

namespace Reminders.Business.Contracts.Entity
{
    public interface IReminderRepository 
        : IEntityRepositoryGeneric<ReminderEntity>
    {
    }
}
