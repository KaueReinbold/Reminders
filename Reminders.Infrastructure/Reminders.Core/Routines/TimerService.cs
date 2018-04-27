using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reminders.Core.Routines
{
    public abstract class TimerService : IHostedService, IDisposable
    {
        private Timer _timer;

        protected ILogger<TimerService> _logger;
        protected TimeSpan DueTime { get; set; }
        protected TimeSpan Period { get; set; }

        public TimerService(ILogger<TimerService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, DueTime, Period);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public abstract void DoWork(Object stateInfo);
    }
}
