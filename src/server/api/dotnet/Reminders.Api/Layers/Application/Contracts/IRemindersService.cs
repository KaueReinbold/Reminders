namespace Reminders.Application.Contracts;

public interface IRemindersService
{
    Task<ReminderViewModel> InsertAsync(ReminderViewModel reminderViewModel);
    Task<ReminderViewModel> EditAsync(Guid id, ReminderViewModel reminderViewModel);
    Task DeleteAsync(Guid id);

    IQueryable<ReminderViewModel> Get();
    Task<ReminderViewModel> GetAsync(Guid id);
}
