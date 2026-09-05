namespace Reminders.Application.Validators.Reminders;

public class ReminderViewModelValidator
    : AbstractValidator<ReminderViewModel>
{
    public ReminderViewModelValidator()
    {
        // Create only: an overdue reminder must stay editable, so the update path
        // does not revalidate a date that is already stored (ADR-0011).
        RuleSet("Insert", () =>
        {
            RuleFor(reminder => reminder.IsDone)
                .Equal(false)
                .OverridePropertyName("isDone")
                .WithMessage(RemindersResources.InvalidIsDone);

            RuleFor(reminder => reminder.LimitDate)
                .Must(limitDate => limitDate.UtcDateTime.Date > DateTime.UtcNow.Date)
                .OverridePropertyName("limitDate")
                .WithMessage(RemindersResources.InvalidLimitDate);
        });
    }
}
