using System;
using Microsoft.Extensions.Logging;
using Reminders.Business.BusinessModels;

namespace Reminders.Core.Routines.Reminders
{
    public class ReminderCompleteService : TimerService
    {
        private BusinessReminderModel _businessReminderModel;

        public ReminderCompleteService(ILogger<ReminderCompleteService> logger, BusinessReminderModel businessReminderModel) : base(logger)
        {
            DueTime = TimeSpan.Zero;
            Period = TimeSpan.FromMinutes(30);

            _businessReminderModel = businessReminderModel;
        }

        public override void DoWork(object stateInfo)
        {
            try
            {
                var reminders = _businessReminderModel.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
