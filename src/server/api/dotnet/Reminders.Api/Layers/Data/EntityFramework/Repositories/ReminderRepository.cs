namespace Reminders.Infrastructure.Data.EntityFramework.Repositories;

public class RemindersRepository
    : Repository<Reminder>, IRemindersRepository
{
    public RemindersRepository(IUnitOfWork unitOfWork)
        : base(unitOfWork) { }
}