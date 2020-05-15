using System;
using System.Collections.Generic;
using System.Linq;
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
        protected Mock<IRemindersRepository> repositoryMock;
        protected Mock<IUnitOfWork> unitOfWorkMock;
        protected IMapper mapperMock;

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
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

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
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            var exception = Assert.ThrowsException<ValidationException>(() => service.Insert(reminderViewModel));

            // assert
            Assert.IsTrue(exception.Message.Contains(RemindersResources.InvalidLimitDate));
        }

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
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            var result = service.Get();

            // assert
            repositoryMock.Verify(repository => repository.Get(), Times.Once);
            Assert.IsTrue(result.All(r => r.Title == reminder.Title));
        }

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

            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            // act
            var result = service.Get(reminder.Id);

            // assert
            Assert.AreEqual(reminder.Title, result.Title);
        }

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
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            service.Insert(reminder);

            // assert
            repositoryMock.Verify(repository =>
                repository.Add(It.Is<Reminder>(r =>
                    r.Title == reminder.Title &&
                    r.Description == reminder.Description &&
                    r.LimitDate == reminder.LimitDate)), Times.Once);
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(), Times.Once);
        }

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
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

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
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            var exception = Assert.ThrowsException<ValidationException>(() => service.Edit(reminder.Id, reminder));

            // assert
            Assert.IsTrue(exception.Message.Contains(RemindersResources.InvalidLimitDate));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_NotFindOnEdit()
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
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            var exception = Assert.ThrowsException<RemindersApplicationException>(() =>
               service.Edit(reminder.Id, reminder));

            // assert
            Assert.IsTrue(exception.StatusCode == ValidationStatus.NotFound);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.NotFound));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_IdsMatchOnEdit()
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
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            var exception = Assert.ThrowsException<RemindersApplicationException>(() =>
               service.Edit(Guid.NewGuid(), reminder));

            // assert
            Assert.IsTrue(exception.StatusCode == ValidationStatus.IdsDoNotMatch);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.IdsDoNotMatch));
        }

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
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

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
        public void Should_NotFindOnDelete()
        {
            // arrange
            repositoryMock
                .Setup(repository => repository.Exists(It.IsAny<Guid>()))
                .Returns(false);

            // act
            var service = new RemindersService(mapperMock, repositoryMock.Object, unitOfWorkMock.Object);

            var exception = Assert.ThrowsException<RemindersApplicationException>(() =>
               service.Delete(Guid.NewGuid()));

            // assert
            Assert.IsTrue(exception.StatusCode == ValidationStatus.NotFound);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.NotFound));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_CreateReminderInstance()
        {
            // arrange, act
            var reminder = new Reminder();

            // assert
            Assert.IsInstanceOfType(reminder, typeof(Reminder));
        }
    }
}