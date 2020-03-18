using FluentValidation;
using Reminders.Application.Validators.Reminders.Resources;
using Reminders.Domain.Models;
using System;

namespace Reminders.Application.Validators.Reminders
{
    public class ReminderValidator
        : AbstractValidator<Reminder>
    {
        public ReminderValidator()
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
}
