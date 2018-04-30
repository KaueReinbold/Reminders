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

namespace Reminders.Business.Test.Reminders
{
    [TestClass]
    public class RemindersRepositoryEntityTests : StartupBusinessTest
    {
        private RepositoryReminderEntity _repositoryReminderEntity;
        private string _testGuid;

        public RemindersRepositoryEntityTests()
        {            
            var reminderDbContext = _serviceProvider.GetService<RemindersDbContext>();
            
            _repositoryReminderEntity = new RepositoryReminderEntity(reminderDbContext);

            _testGuid = Guid.NewGuid().ToString();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _repositoryReminderEntity = null;
        }

        [TestMethod]
        public void RemindersRepositoryCRUD()
        {
            RemindersInsert();

            RemindersEdit();

            RemindersDelete();
        }

        public void RemindersInsert()
        {

            var reminder = new ReminderEntity
            {
                Title = $"Repository Test - Title - {_testGuid}",
                Description = $"Repository Test - Description - {_testGuid}",
                LimitDate = DateTime.UtcNow.AddDays(10),
                IsDone = false
            };

            reminder = _repositoryReminderEntity.Insert(reminder);

            if (reminder.Id == 0)
                Assert.Fail("The insert has not happened!");
        }

        public void RemindersEdit()
        {
            var success = false;

            var reminder = _repositoryReminderEntity.GetAll(r => r.Title.Contains(_testGuid)).FirstOrDefault();

            if (reminder != null)
            {
                reminder.Title = $"Repository Test - Title Edited - {_testGuid}";
                reminder.Description = $"Repository Test - Description - {_testGuid}";
                reminder.LimitDate = DateTime.UtcNow.AddDays(5);
                reminder.IsDone = true;

                success = _repositoryReminderEntity.Update(reminder);
            }

            if (!success)
                Assert.Fail("The update has not happened!");
        }

        public void RemindersDelete()
        {
            var success = false;

            var reminders = _repositoryReminderEntity.GetAll(r => r.Title.Contains(_testGuid));

            if (reminders.Any())
            {
                foreach (var reminder in reminders)
                {
                    success = _repositoryReminderEntity.Delete(reminder.Id);

                    if (!success)
                        break;
                }
            }

            if (!success)
                Assert.Fail("The delete has not happened!");
        }
    }
}
