using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Business.RepositoryEntities;
using Reminders.Context.RemindersContext;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;
using System.IO;

namespace Reminders.Mvc.Test.Business
{
    public class TestConfiguration
    {
        private IConfigurationRoot _configuration;
        public ServiceProvider _serviceProvider;
        public IMapper _mapper;
        
        public TestConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IRepositoryEntityGeneric<ReminderEntity>, RepositoryReminderEntity>();

            serviceCollection.AddSingleton<IBusinessModelGeneric<ReminderModel>, BusinessReminderModel>();

            serviceCollection.AddDbContext<RemindersDbContext>(options =>
                      options.UseSqlServer(_configuration.GetConnectionString("StringConnectionReminders")));

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
