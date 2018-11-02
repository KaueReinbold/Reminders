using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Business.BusinessModels;
using Reminders.Business.Contracts;
using Reminders.Business.Contracts.Entity;
using Reminders.Business.RepositoryEntities;
using Reminders.Business.RepositoryEntities.Persistence;
using Reminders.Business.RepositoryEntities.Persistence.Repositories;
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
        private IUnitOfWork _unitOfWork;
        private string _testGuid;

        public RemindersRepositoryEntityTests()
        {            
            var reminderDbContext = _serviceProvider.GetService<RemindersDbContext>();
            
            _unitOfWork = new UnitOfWork(reminderDbContext);

            _testGuid = Guid.NewGuid().ToString();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _unitOfWork = null;
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

            _unitOfWork.RemindersRepository.Add(reminder);

            _unitOfWork.Complete();

            if (reminder.Id == 0)
                Assert.Fail("The insert has not happened!");
        }

        public void RemindersEdit()
        {
            var success = false;

            var reminder = _unitOfWork.RemindersRepository.GetAll().FirstOrDefault(r => r.Title.Contains(_testGuid));

            if (reminder != null)
            {
                reminder.Title = $"Repository Test - Title Edited - {_testGuid}";
                reminder.Description = $"Repository Test - Description - {_testGuid}";
                reminder.LimitDate = DateTime.UtcNow.AddDays(5);
                reminder.IsDone = true;

                success = _unitOfWork.Complete() > 0;
            }

            if (!success)
                Assert.Fail("The update has not happened!");
        }

        public void RemindersDelete()
        {
            var success = false;

            var reminders = _unitOfWork.RemindersRepository.GetAll().Where(r => r.Title.Contains(_testGuid));

            if (reminders.Any())
            {
                _unitOfWork.RemindersRepository.RemoveRange(reminders);

                success = _unitOfWork.Complete() > 0;
            }

            if (!success)
                Assert.Fail("The delete has not happened!");
        }
    }
}
