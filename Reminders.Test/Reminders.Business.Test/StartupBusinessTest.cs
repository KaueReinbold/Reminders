using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Business.RepositoryEntities;
using Reminders.Context.RemindersContext;
using Reminders.Core.Options;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;
using System.IO;

namespace Reminders.Business.Test
{
    public class StartupBusinessTest
    {
        private IConfigurationRoot _configuration;
        public ServiceProvider _serviceProvider;
        public IMapper _mapper;
        public static readonly LoggerFactory DebugLoggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider((_, __) => true) });

        public StartupBusinessTest()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IRepositoryEntityGeneric<ReminderEntity>, RepositoryReminderEntity>();

            serviceCollection.AddSingleton<IBusinessModelGeneric<ReminderModel>, BusinessReminderModel>();

            serviceCollection.AddDbContext<RemindersDbContext>(options =>
                      options.UseLoggerFactory(DebugLoggerFactory).UseSqlServer(_configuration.GetConnectionString("StringConnectionReminders")));

            serviceCollection.AddOptions();

            serviceCollection.Configure<TestOptions>(_configuration.GetSection("TestConfiguration"));

            serviceCollection.AddLogging();

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _mapper = new MapperConfiguration(configuration =>
            {
                configuration.CreateMap<ReminderModel, ReminderEntity>();
                configuration.CreateMap<ReminderEntity, ReminderModel>();
            }).CreateMapper();
        }
    }
}
