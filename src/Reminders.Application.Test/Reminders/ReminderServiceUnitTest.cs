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
            Assert.AreEqual(title, result.Title);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataToAdd), DynamicDataSourceType.Property)]
        public void Should_Insert_Reminder(
            string title, string description, DateTime limitDate, bool isDone)
        {
            // arrange
            var result = false;

            repositoryMock
                .Setup(r => r.Add(It.IsAny<Reminder>()));

            try
            {
                // act
                var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

                service.Insert(new ReminderViewModel()
                {
                    Title = title,
                    Description = description,
                    LimitDate = limitDate,
                    IsDone = isDone
                });

                result = true;
            }
            catch (Exception) { }

            // assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataToEdit), DynamicDataSourceType.Property)]
        public void Should_Edit_Reminder(
            Guid id, string title, string description, DateTime limitDate, bool isDone)
        {
            // arrange
            var result = false;

            repositoryMock
                .Setup(r => r.Update(It.IsAny<Reminder>()));

            try
            {
                // act
                var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

                service.Edit(id, new ReminderViewModel()
                {
                    Id = id,
                    Title = title,
                    Description = description,
                    LimitDate = limitDate,
                    IsDone = isDone
                });

                result = true;
            }
            catch (Exception) { }

            // assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataToEdit), DynamicDataSourceType.Property)]
        public void Should_Throw_Exception_With_Message_On_Edit_Reminder(
            Guid id, string title, string description, DateTime limitDate, bool isDone)
        {
            // arrange
            var result = true;

            repositoryMock
                .Setup(r => r.Update(It.IsAny<Reminder>()));

            try
            {
                // act
                var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

                service.Edit(Guid.NewGuid(), new ReminderViewModel()
                {
                    Id = id,
                    Title = title,
                    Description = description,
                    LimitDate = limitDate,
                    IsDone = isDone
                });

                result = false;
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "Ids must match");
            }
            catch (Exception) { result = false; }

            // assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DynamicData(nameof(DataToWithTitle), DynamicDataSourceType.Property)]
        public void Should_Delete_Reminder(
            string title)
        {
            // arrange
            var result = false;
            var reminder = ReminderMock.Reminders.First(r => r.Title == title);

            repositoryMock
                .Setup(r => r.Remove(It.IsAny<Guid>()));

            try
            {
                // act
                var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

                service.Delete(reminder.Id);

                result = true;
            }
            catch (Exception) { }

            // assert
            Assert.IsTrue(result);
        }

        public static IEnumerable<object[]> DataToWithTitle => 
            ReminderMock.Reminders.Select(reminder => new object[] { reminder.Title }).ToList();

        public static IEnumerable<object[]> DataToAdd =>
            ReminderMock.Reminders.Select(reminder => new object[]
            {
                reminder.Title,
                reminder.Description,
                reminder.LimitDate,
                reminder.IsDone
            }).ToList();

        public static IEnumerable<object[]> DataToEdit =>
            ReminderMock.Reminders.Select(reminder => new object[]
            {
                reminder.Id,
                string.Concat(reminder.Title, " - Edited"),
                reminder.Description,
                reminder.LimitDate,
                reminder.IsDone
            }).ToList();
    }
}
