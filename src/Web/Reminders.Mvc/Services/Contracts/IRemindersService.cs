namespace Reminders.Mvc.Services.Contracts;

public interface IRemindersService
{
    Task<IEnumerable<ReminderViewModel>?> GetRemindersAsync();
    Task<ReminderViewModel?> GetReminderAsync(Guid id);
    Task AddReminderAsync(ReminderViewModel reminderViewModel);
    Task EditReminderAsync(Guid id, ReminderViewModel reminderViewModel);
    Task DeleteReminderAsync(Guid id);
}
