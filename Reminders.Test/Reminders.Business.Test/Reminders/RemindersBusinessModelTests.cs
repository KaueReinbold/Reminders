using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;
using System;
using System.Linq;

namespace Reminders.Business.Test.Reminders
{
    [TestClass]
    public class RemindersBusinessModelTests : StartupBusinessTest
    {
        private BusinessReminderModel _businessReminderModel;
        private string _testGuid;

        public RemindersBusinessModelTests()
        {
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var logger = _serviceProvider.GetService<ILogger<BusinessReminderModel>>();

            _businessReminderModel = new BusinessReminderModel(_mapper, logger, unitOfWork);

            _testGuid = Guid.NewGuid().ToString();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _businessReminderModel = null;
        }

        [TestMethod]
        public void RemindersBusinessCRUD()
        {
            RemindersInsert();

            RemindersEdit();

            RemindersDelete();
        }

        public void RemindersInsert()
        {

            var reminder = new ReminderModel
            {
                Title = $"Business Test - Title - {_testGuid}",
                Description = $"Business Test - Description - {_testGuid}",
                LimitDate = DateTime.UtcNow.AddDays(10),
                IsDone = false
            };

            reminder = _businessReminderModel.Insert(reminder);

            if (reminder.Id == 0)
                Assert.Fail("The insert has not happened!");
        }
        
        public void RemindersEdit()
        {
            var success = false;

            var reminder = _businessReminderModel.GetAll().Where(r => r.Title.Contains(_testGuid)).FirstOrDefault();

            if (reminder != null)
            {

                reminder.Title = $"Business Test - Title Edited - {_testGuid}";
                reminder.Description = $"Business Test - Description - {_testGuid}";
                reminder.LimitDate = DateTime.UtcNow.AddDays(5);
                reminder.IsDone = true;

                success = _businessReminderModel.Update(reminder);
            }

            if (!success)
                Assert.Fail("The update has not happened!");
        }
        
        public void RemindersDelete()
        {
            var success = false;

            var reminders = _businessReminderModel.GetAll().Where(r => r.Title.Contains(_testGuid)).ToList();

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
