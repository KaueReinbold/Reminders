using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reminders.Application.Services;
using Reminders.Application.Test.MockData;
using Reminders.Application.ViewModels;
using Reminders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reminders.Application.Test
{
    [TestClass]
    public class ReminderServiceUnitTest
        : BaseUnitTest
    {
        [TestMethod]
        public void Should_Get_Reminder()
        {
            // arrange
            repositoryMock
                .Setup(r => r.Get())
                .Returns(ReminderMock.Reminders);

            // act
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            var result = service.Get();

            // assert
            repositoryMock.Verify(r => r.Get(), Times.Once);
            Assert.AreEqual(ReminderMock.Reminders.Count(), result.Count());
        }

        [DataTestMethod]
        [DynamicData(nameof(DataToWithTitle), DynamicDataSourceType.Property)]
        public void Should_Get_Reminder_By_Id(string title)
        {
            // arrange
            var expectedResult = ReminderMock.Reminders.First(r => r.Title == title);

            repositoryMock
                .Setup(r => r.Get(It.IsAny<Guid>()))
                .Returns(expectedResult);

            // act
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            var result = service.Get(expectedResult.Id);

            // assert
            repositoryMock.Verify(r => r.Get(It.IsAny<Guid>()), Times.Once);
            Assert.AreEqual(title, result.Title);
        }

        [TestMethod]
        public void Should_Insert_Reminder()
        {
            // arrange
            repositoryMock
                .Setup(r => r.Add(It.IsAny<Reminder>()));

            // act
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            service.Insert(new ReminderViewModel()
            {
                Title = "Title",
                Description = "Description",
                LimitDate = new DateTime(),
                IsDone = false
            });

            // assert
            repositoryMock.Verify(r => r.Add(It.IsAny<Reminder>()), Times.Once);
            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [TestMethod]
        public void Should_Edit_Reminder()
        {
            // arrange

            repositoryMock
                .Setup(r => r.Update(It.IsAny<Reminder>()));

            // act
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            var id = Guid.NewGuid();

            service.Edit(id, new ReminderViewModel()
            {
                Id = id,
                Title = "Title",
                Description = "Description",
                LimitDate = new DateTime(),
                IsDone = false
            });

            // assert
            repositoryMock.Verify(r => r.Update(It.IsAny<Reminder>()), Times.Once);
            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [TestMethod]
        public void Should_Throw_Exception_On_Edit_Reminder()
        {
            // arrange
            repositoryMock
                .Setup(r => r.Update(It.IsAny<Reminder>()));

            // act
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            // assert
            Assert.ThrowsException<ArgumentException>(() =>
                service.Edit(Guid.NewGuid(), new ReminderViewModel()
                {
                    Id = Guid.Empty,
                    Title = "Title",
                    Description = "Description",
                    LimitDate = new DateTime(),
                    IsDone = false
                }));
        }

        [DataTestMethod]
        [DynamicData(nameof(DataToWithTitle), DynamicDataSourceType.Property)]
        public void Should_Delete_Reminder(string title)
        {
            // arrange
            var reminder = ReminderMock.Reminders.First(r => r.Title == title);

            repositoryMock
                .Setup(r => r.Remove(It.IsAny<Guid>()));

            // act
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            service.Delete(reminder.Id);

            // assert
            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        public static IEnumerable<object[]> DataToWithTitle =>
            ReminderMock.Reminders.Select(reminder => new object[] { reminder.Title }).ToList();

    }
}
