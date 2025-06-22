namespace Reminders.Application.Contracts;

public interface IRemindersBlockchainService
{
    Task<string> CreateReminderAsync(string text);
    Task<GetReminderOutput> GetReminderAsync(int id);
    Task<string> UpdateReminderAsync(int id, string text);
    Task<string> DeleteReminderAsync(int id);
    Task<int> GetReminderCountAsync();
}