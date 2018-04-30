using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Domain.Entities;
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
                    var businessModelGeneric = scope.ServiceProvider.GetRequiredService<IBusinessModelGeneric<ReminderModel>>();

                    var reminders = businessModelGeneric
                                        .GetAll()
                                        .Where(reminder => !reminder.IsDone && reminder.LimitDate < DateTime.UtcNow)
                                        .ToList();

                    reminders.ForEach(reminder =>
                    {
                        reminder.IsDone = true;

                        businessModelGeneric.Update(reminder);
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
