namespace Reminders.Application.Validators.Reminders;

public class ReminderViewModelValidator
    : AbstractValidator<ReminderViewModel>
{
    public ReminderViewModelValidator()
    {
        RuleSet("Insert", () =>
        {
            RuleFor(reminder => reminder.IsDone)
                .Equal(false)
                .WithMessage(RemindersResources.InvalidIsDone);
        });

        RuleFor(reminder => reminder.LimitDate.Date)
            .GreaterThan(DateTime.UtcNow.Date)
            .WithMessage(RemindersResources.InvalidLimitDate);
    }
}