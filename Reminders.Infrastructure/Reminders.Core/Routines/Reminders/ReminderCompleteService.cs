using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;

namespace Reminders.Core.Routines.Reminders
{
    public class ReminderCompleteService : TimerService
    {
        private IRepositoryEntityGeneric<ReminderEntity> _repositoryEntityReminders;

        public ReminderCompleteService(ILogger<ReminderCompleteService> logger, IRepositoryEntityGeneric<ReminderEntity> repositoryEntityReminders) : base(logger)
        {
            DueTime = TimeSpan.Zero;
            Period = TimeSpan.FromMinutes(2);

            _repositoryEntityReminders = repositoryEntityReminders;
        }

        public override void DoWork(object stateInfo)
        {
            try
            {
                var reminders = _repositoryEntityReminders.GetAll().Where(reminder => !reminder.IsDone && reminder.LimitDate < DateTime.UtcNow).ToList();

                reminders.ForEach(reminder =>
                {
                    reminder.IsDone = true;

                    _repositoryEntityReminders.Update(reminder);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
