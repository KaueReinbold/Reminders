using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reminders.Application.Mapper.Extensions;
using Reminders.Application.Services;
using Reminders.Application.Validators.Reminders.Exceptions;
using Reminders.Application.Validators.Reminders.Exceptions.Enumerables;
using Reminders.Application.Validators.Reminders.Resources;
using Reminders.Application.ViewModels;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;

namespace Reminders.Application.Test
{
    [TestClass]
    public class ReminderServiceUnitTest
    {

        #region Variables

        protected Mock<IRemindersRepository> repositoryMock;
        protected Mock<IUnitOfWork> unitOfWorkMock;
        protected IMapper mapperMock;

        #endregion

        #region Initialize

        [TestInitialize]
        public void TestInitialize()
        {
            repositoryMock = new Mock<IRemindersRepository>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            mapperMock = AutoMapperConfiguration.CreateMapper();

            unitOfWorkMock
                .Setup(u => u.Commit())
                .Returns(true);
        }

        private RemindersService GetRemindersService() =>
            new RemindersService(
                mapperMock,
                repositoryMock.Object,
                unitOfWorkMock.Object);

        #endregion

        #region Insert

        [Timeout(1000)]
        [TestMethod]
        public void Should_Insert()
        {
            // arrange
            var reminder = new ReminderViewModel()
            {
                Title = "Title",
                Description = "Description",
                LimitDate = DateTime.UtcNow.AddDays(1)
            };

            // act
            var service = GetRemindersService();

            service.Insert(reminder);

            // assert
            repositoryMock.Verify(repository =>
                repository.Add(It.Is<Reminder>(r =>
                    r.Title == reminder.Title &&
                    r.Description == reminder.Description &&
                    r.LimitDate == reminder.LimitDate &&
                    !r.IsDone)), Times.Once);
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(), Times.Once);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_IsDoneAsFalseOnInsert()
        {
            // arrange
            var reminderViewModel = new ReminderViewModel
            {
                Title = "My Title",
                Description = "My Description",
                LimitDate = DateTime.UtcNow.AddDays(1),
                IsDone = true
            };

            // act
            var service = GetRemindersService();

            service.Insert(reminderViewModel);

            // assert
            repositoryMock.Verify(repository =>
                repository.Add(It.Is<Reminder>(reminder => !reminder.IsDone)),
                Times.Once);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_LimitDateAfterTodayOnInsert()
        {
            // arrange
            var reminderViewModel = new ReminderViewModel
            {
                Title = "My Title",
                Description = "My Description",
                LimitDate = DateTime.UtcNow
            };

            // act
            var service = GetRemindersService();

            var exception = Assert.ThrowsException<ValidationException>(() => service.Insert(reminderViewModel));

            // assert
            Assert.IsTrue(exception.Message.Contains(RemindersResources.InvalidLimitDate));
        }

        #endregion

        #region Edit

        [Timeout(1000)]
        [TestMethod]
        public void Should_Edit()
        {
            // arrange
            var reminder = new ReminderViewModel()
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                LimitDate = DateTime.UtcNow.AddDays(1)
            };

            repositoryMock
                .Setup(repository => repository.Exists(It.IsAny<Guid>()))
                .Returns(true);

            // act
            var service = GetRemindersService();

            service.Edit(reminder.Id, reminder);

            // assert
            repositoryMock.Verify(repository =>
                repository.Update(It.Is<Reminder>(r =>
                    r.Title == reminder.Title &&
                    r.Description == reminder.Description &&
                    r.LimitDate == reminder.LimitDate)), Times.Once);
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(), Times.Once);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_LimitDateAfterTodayOnEdit()
        {
            // arrange
            var reminder = new ReminderViewModel()
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                LimitDate = DateTime.UtcNow.AddDays(-1)
            };

            // act
            var service = GetRemindersService();

            var exception = Assert.ThrowsException<ValidationException>(() => service.Edit(reminder.Id, reminder));

            // assert
            Assert.IsTrue(exception.Message.Contains(RemindersResources.InvalidLimitDate));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_NotFoundOnEdit()
        {
            // arrange
            var reminder = new ReminderViewModel()
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                LimitDate = DateTime.UtcNow.AddDays(1)
            };

            repositoryMock
                .Setup(repository => repository.Exists(It.IsAny<Guid>()))
                .Returns(false);

            // act
            var service = GetRemindersService();

            var exception = Assert.ThrowsException<RemindersApplicationException>(() =>
               service.Edit(reminder.Id, reminder));

            // assert
            Assert.IsTrue(exception.StatusCode == ValidationStatus.NotFound);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.NotFound));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_IdsDoNotMatchOnEdit()
        {
            // arrange
            var reminder = new ReminderViewModel()
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                LimitDate = DateTime.UtcNow.AddDays(1)
            };

            repositoryMock
                .Setup(repository => repository.Exists(It.IsAny<Guid>()))
                .Returns(false);

            // act
            var service = GetRemindersService();

            var exception = Assert.ThrowsException<RemindersApplicationException>(() =>
               service.Edit(Guid.NewGuid(), reminder));

            // assert
            Assert.IsTrue(exception.StatusCode == ValidationStatus.IdsDoNotMatch);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.IdsDoNotMatch));
        }

        #endregion

        #region Delete

        [Timeout(1000)]
        [TestMethod]
        public void Should_Delete()
        {
            // arrange
            var reminder = new Reminder("My Title", "My Description", DateTime.UtcNow, false);

            repositoryMock
                .Setup(repository => repository.Exists(It.IsAny<Guid>()))
                .Returns(true);

            repositoryMock
                .Setup(repository => repository.Get(It.IsAny<Guid>()))
                .Returns(reminder);

            // act
            var service = GetRemindersService();

            service.Delete(reminder.Id);

            // assert
            repositoryMock
                .Verify(repository =>
                    repository.Update(It.Is<Reminder>(r =>
                        r.Id == reminder.Id &&
                        r.Title == reminder.Title &&
                        r.Description == reminder.Description &&
                        r.LimitDate == reminder.LimitDate &&
                        r.IsDeleted)));
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(), Times.Once);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_NotFoundOnDelete()
        {
            // arrange
            repositoryMock
                .Setup(repository => repository.Exists(It.IsAny<Guid>()))
                .Returns(false);

            // act
            var service = GetRemindersService();

            var exception = Assert.ThrowsException<RemindersApplicationException>(() =>
               service.Delete(Guid.NewGuid()));

            // assert
            Assert.IsTrue(exception.StatusCode == ValidationStatus.NotFound);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.NotFound));
        }

        #endregion

        #region Get

        [Timeout(1000)]
        [TestMethod]
        public void Should_Get()
        {
            // arrange
            var reminder = new Reminder("My Title", "My Description", DateTime.UtcNow, false);

            repositoryMock
                .Setup(repository => repository.Get())
                .Returns(new List<Reminder>
                {
                    reminder
                }.AsQueryable());

            // act
            var service = GetRemindersService();

            var result = service.Get();

            // assert
            repositoryMock.Verify(repository => repository.Get(), Times.Once);
            Assert.IsTrue(result.All(r => r.Title == reminder.Title));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_GetZero()
        {
            // arrange
            var reminder = new Reminder("My Title", "My Description", DateTime.UtcNow, false);

            reminder.Delete();

            repositoryMock
                .Setup(repository => repository.Get())
                .Returns(new List<Reminder>
                {
                    reminder
                }.AsQueryable());

            // act
            var service = GetRemindersService();

            var result = service.Get();

            // assert
            repositoryMock.Verify(repository => repository.Get(), Times.Once);
            Assert.IsTrue(result.Count(r => r.Title == reminder.Title) == 0);
        }

        #endregion

        #region Get By Id

        [Timeout(1000)]
        [TestMethod]
        public void Should_GetById()
        {
            // arrange
            var reminder = new Reminder("My Title", "My Description", DateTime.UtcNow, false);

            repositoryMock
                .Setup(repository => repository.Get())
                .Returns(new List<Reminder>
                {
                    reminder
                }.AsQueryable());

            var service = GetRemindersService();

            // act
            var result = service.Get(reminder.Id);

            // assert
            Assert.AreEqual(reminder.Title, result.Title);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_NotGetById()
        {
            // arrange
            var reminder = new Reminder("My Title", "My Description", DateTime.UtcNow, false);

            reminder.Delete();

            repositoryMock
                .Setup(repository => repository.Get())
                .Returns(new List<Reminder>
                {
                    reminder
                }.AsQueryable());

            var service = GetRemindersService();

            // act
            var result = service.Get(reminder.Id);

            // assert
            Assert.IsNull(result);
        }

        #endregion

        #region Instance

        [Timeout(1000)]
        [TestMethod]
        public void Should_CreateReminderInstance()
        {
            // arrange, act
            var reminder = new Reminder();

            // assert
            Assert.IsInstanceOfType(reminder, typeof(Reminder));
        }

        #endregion
    }
}