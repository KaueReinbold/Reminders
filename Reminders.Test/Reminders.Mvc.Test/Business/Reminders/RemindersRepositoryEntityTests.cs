using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Business.RepositoryEntities;
using Reminders.Context.RemindersContext;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;
using System;
using System.Linq;

namespace Reminders.Mvc.Test.Business.Reminders
{
    [TestClass]
    public class RemindersRepositoryEntityTests : TestConfigurationBusiness
    {
        private RepositoryReminderEntity _repositoryReminderEntity;

        public RemindersRepositoryEntityTests()
        {            
            var reminderDbContext = _serviceProvider.GetService<RemindersDbContext>();
            
            _repositoryReminderEntity = new RepositoryReminderEntity(reminderDbContext);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _repositoryReminderEntity = null;
        }

        [TestMethod]
        public void RemindersInsert()
        {
            var newGuid = Guid.NewGuid().ToString();

            var reminder = new ReminderEntity
            {
                Title = $"Repository Test - Title - {newGuid}",
                Description = $"Repository Test - Description - {newGuid}",
                LimitDate = DateTime.UtcNow.AddDays(10),
                IsDone = false
            };

            reminder = _repositoryReminderEntity.Insert(reminder);

            if (reminder.Id == 0)
                Assert.Fail("The insert has not happened!");
        }

        [TestMethod]
        public void RemindersEdit()
        {
            var success = false;

            var reminder = _repositoryReminderEntity.GetAll(r => r.Title.StartsWith("Repository Test")).FirstOrDefault();

            if (reminder != null)
            {
                var newGuid = Guid.NewGuid().ToString();
                reminder.Title = $"Repository Test - Title Edited - {newGuid}";
                reminder.Description = $"Repository Test - Description - {newGuid}";
                reminder.LimitDate = DateTime.UtcNow.AddDays(5);
                reminder.IsDone = true;

                success = _repositoryReminderEntity.Update(reminder);
            }

            if (!success)
                Assert.Fail("The update has not happened!");
        }

        [TestMethod]
        public void RemindersDelete()
        {
            var success = false;

            var reminders = _repositoryReminderEntity.GetAll(r => r.Title.Contains("Repository Test"));

            if (reminders.Any())
            {
                foreach (var reminder in reminders)
                {
                    success = _repositoryReminderEntity.Delete(reminder);

                    if (!success)
                        break;
                }
            }

            if (!success)
                Assert.Fail("The delete has not happened!");
        }
    }
}
