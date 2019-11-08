using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reminders.Application.Contracts;
using Reminders.Application.Services;
using Reminders.Application.ViewModels;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Reminders.Application.Test
{
    [TestClass]
    public class ReminderServiceUnitTest
    {


        [TestMethod]
        public void Should_Get_Reminder()
        {
            // arrange
            var mapperMock = new Mock<IMapper>();
            var repositoryMock = new Mock<IRemindersRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            mapperMock.Setup(m => m.Map<Reminder, ReminderViewModel>(It.IsAny<Reminder>())).Returns(new ReminderViewModel());
            mapperMock.Setup(m => m.Map<ReminderViewModel, Reminder>(It.IsAny<ReminderViewModel>())).Returns(new Reminder());
            repositoryMock.Setup(r => r.Get()).Returns(ReminderMock.Reminders);
            unitOfWorkMock.Setup(u => u.Commit()).Returns(true);

            // act
            var service = new RemindersService(mapperMock.Object, repositoryMock.Object, unitOfWorkMock.Object);

            var result = service.Get();

            // assert
            Assert.AreEqual(ReminderMock.Reminders, result);
        }
    }

    public static class ReminderMock
    {
        public static IQueryable<Reminder> Reminders => new List<Reminder> { new Reminder { } }.AsQueryable();
    }
}
