namespace Reminders.Mvc.Services.Contracts;

public interface IRemindersService
{
    Task<IEnumerable<ReminderViewModel>?> GetRemindersAsync(CancellationToken cancellationToken);
    Task<ReminderViewModel?> GetReminderAsync(Guid id, CancellationToken cancellationToken);
    Task AddReminderAsync(ReminderViewModel reminderViewModel, CancellationToken cancellationToken);
    Task EditReminderAsync(Guid id, ReminderViewModel reminderViewModel, CancellationToken cancellationToken);
    Task DeleteReminderAsync(Guid id, CancellationToken cancellationToken);
}
