using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;
using System;
using System.Linq;

namespace Reminders.Mvc.Test.Business.Reminders
{
    [TestClass]
    public class RemindersBusinessModelTests : TestConfiguration
    {
        private BusinessReminderModel _businessReminderModel;

        public RemindersBusinessModelTests()
        {            
            var repositoryRemindersEntity = _serviceProvider.GetService<IRepositoryEntityGeneric<ReminderEntity>>();

            var logger = _serviceProvider.GetService<ILogger<BusinessReminderModel>>();

            _businessReminderModel = new BusinessReminderModel(_mapper, logger, repositoryRemindersEntity);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _businessReminderModel = null;
        }

        [TestMethod]
        public void RemindersInsert()
        {
            var newGuid = Guid.NewGuid().ToString();

            var reminder = new ReminderModel
            {
                Title = $"Business Test - Title - {newGuid}",
                Description = $"Business Test - Description - {newGuid}",
                LimitDate = DateTime.Now.AddDays(10),
                IsDone = false
            };

            reminder = _businessReminderModel.Insert(reminder);

            if (reminder.Id == 0)
                Assert.Fail("The insert has not happened!");
        }

        [TestMethod]
        public void RemindersEdit()
        {
            var success = false;

            var reminder = _businessReminderModel.GetAll().FirstOrDefault(r => r.Title.StartsWith("Business Test"));

            if (reminder != null)
            {
                var newGuid = Guid.NewGuid().ToString();
                reminder.Title = $"Business Test - Title Edited - {newGuid}";
                reminder.Description = $"Business Test - Description - {newGuid}";
                reminder.LimitDate = DateTime.Now.AddDays(5);
                reminder.IsDone = true;

                success = _businessReminderModel.Update(reminder);
            }

            if (!success)
                Assert.Fail("The update has not happened!");
        }

        [TestMethod]
        public void RemindersDelete()
        {
            var success = false;

            var reminders = _businessReminderModel.GetAll().Where(r => r.Title.Contains("Business Test")).ToList();

            if (reminders.Any())
            {
                foreach (var reminder in reminders)
                {
                    success = _businessReminderModel.Delete(reminder.Id);

                    if (!success)
                        break;
                }
            }

            if (!success)
                Assert.Fail("The delete has not happened!");
        }
    }
}
