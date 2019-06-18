using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reminders.Business.Contracts.Business;
using Reminders.Domain.Models;

namespace Reminders.Core.Routines.Reminders
{
    public class ReminderCompleteService : TimerService
    {
        private IServiceProvider _serviceProvider;

        public ReminderCompleteService(ILogger<ReminderCompleteService> logger, IServiceProvider serviceProvider)
            : base(logger)
        {
            _serviceProvider = serviceProvider;

            DueTime = TimeSpan.Zero;
            Period = TimeSpan.FromMinutes(2);
        }

        public override void DoWork(object stateInfo)
        {
            try
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    var remindersRepository = scope.ServiceProvider.GetRequiredService<IBusinessModelGeneric<ReminderModel>>();

                    var reminders = remindersRepository
                                        .GetAll()
                                        .Where(reminder => !reminder.IsDone && reminder.LimitDate < DateTime.UtcNow)
                                        .ToList();

                    reminders.ForEach(reminder =>
                    {
                        reminder.IsDone = true;

                        remindersRepository.Update(reminder);
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
