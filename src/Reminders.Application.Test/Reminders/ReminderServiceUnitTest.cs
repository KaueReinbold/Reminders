using System;
using System.Linq;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Application.Services;
using Reminders.Application.Validators.Reminders;
using Reminders.Application.Validators.Reminders.Exceptions;
using Reminders.Application.Validators.Reminders.Exceptions.Enumerables;
using Reminders.Application.Validators.Reminders.Resources;
using Reminders.Application.ViewModels;
using Reminders.Domain.Models;

namespace Reminders.Application.Test
{
    [TestClass]
    public class ReminderServiceUnitTest
        : BaseUnitTest
    {

        [Timeout(1000)]
        [TestMethod]
        public void Should_IsDoneAsFalseOnInsert()
        {
            // arrange
            var reminder = new ReminderViewModel
            {
                Title = "My Title",
                Description = "My Description",
                LimitDate = DateTime.UtcNow.AddDays(1),
                IsDone = true
            };
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            service.Insert(reminder);

            // assert
            var result = service.Get().FirstOrDefault(r => r.Title.Equals(reminder.Title));

            Assert.IsFalse(result.IsDone);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_LimitDateAfterTodayOnInsert()
        {
            // arrange
            var reminder = new ReminderViewModel
            {
                Title = "My Title",
                Description = "My Description",
                LimitDate = DateTime.UtcNow.AddDays(-1)
            };
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            // assert
            var exception = Assert.ThrowsException<ValidationException>(() => service.Insert(reminder));

            Assert.IsTrue(exception.Message.Contains(RemindersResources.InvalidLimitDate));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_Get()
        {
            // arrange
            repository.Add(new Reminder("Title", "Description", DateTime.UtcNow, false));

            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            var result = service.Get();

            // assert
            Assert.AreEqual(1, result.Count());
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_GetById()
        {
            // arrange
            var reminder = new Reminder("Title", "Description", DateTime.UtcNow, false);

            repository.Add(reminder);

            var id = repository.Get().FirstOrDefault().Id;

            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            var result = service.Get(id);

            // assert
            Assert.AreEqual(reminder.Title, result.Title);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_Insert()
        {
            // arrange
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);
            var reminder = new ReminderViewModel()
            {
                Title = "Title",
                Description = "Description",
                LimitDate = DateTime.UtcNow.AddDays(1)
            };

            // act
            service.Insert(reminder);

            // assert
            var result = repository.Get().FirstOrDefault();

            Assert.AreEqual(reminder.Title, result.Title);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_Edit()
        {
            // arrange
            repository.Add(new Reminder("Title", "Description", DateTime.UtcNow.AddDays(1), false));

            var id = repository.Get().FirstOrDefault().Id;
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            var reminder = service.Get(id);

            reminder.IsDone = true;

            service.Edit(id, reminder);

            // assert
            var result = repository.Get(id);

            Assert.IsTrue(result.IsDone);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_LimitDateAfterTodayOnEdit()
        {
            // arrange
            repository.Add(new Reminder("Title", "Description", DateTime.UtcNow.AddDays(1), false));

            var id = repository.Get().FirstOrDefault().Id;
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            var reminder = service.Get(id);

            reminder.LimitDate = DateTime.UtcNow;

            // assert
            var exception = Assert.ThrowsException<ValidationException>(() => service.Edit(id, reminder));

            Assert.IsTrue(exception.Message.Contains(RemindersResources.InvalidLimitDate));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_NotFindOnEdit()
        {
            // arrange
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);
            var id = Guid.NewGuid();

            // act
            // assert
            var exception = Assert.ThrowsException<RemindersApplicationException>(() =>
               service.Edit(id, new ReminderViewModel()
               {
                   Id = id,
                   Title = "Title",
                   Description = "Description",
                   LimitDate = DateTime.Now.AddDays(1),
                   IsDone = false
               }));

            Assert.IsTrue(exception.StatusCode == ValidationStatus.NotFound);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.NotFound));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_NotFindDeletedOnEdit()
        {
            // arrange
            repository.Add(new Reminder("Title", "Description", DateTime.UtcNow.AddDays(1), false));

            var id = repository.Get().FirstOrDefault().Id;
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            service.Delete(id);

            // assert
            var exception = Assert.ThrowsException<RemindersApplicationException>(() =>
               service.Edit(id, new ReminderViewModel()
               {
                   Id = id,
                   Title = "Title",
                   Description = "Description",
                   LimitDate = DateTime.Now.AddDays(1),
                   IsDone = false
               }));

            Assert.IsTrue(exception.StatusCode == ValidationStatus.NotFound);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.NotFound));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_IdsMatchOnEdit()
        {
            // arrange
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            // assert
            var exception = Assert.ThrowsException<RemindersApplicationException>(() =>
               service.Edit(Guid.NewGuid(), new ReminderViewModel()
               {
                   Id = Guid.Empty,
                   Title = "Title",
                   Description = "Description",
                   LimitDate = DateTime.Now.AddDays(1),
                   IsDone = false
               }));

            Assert.IsTrue(exception.StatusCode == ValidationStatus.IdsDoNotMatch);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.IdsDoNotMatch));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_Delete()
        {
            // arrange
            repository.Add(new Reminder("Title", "Description", DateTime.UtcNow.AddDays(1), false));

            var id = repository.Get().FirstOrDefault().Id;
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            service.Delete(id);

            // assert
            Assert.IsTrue(repository.Get(id).IsDeleted);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_NotFindOnDelete()
        {
            // arrange
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);
            var id = Guid.NewGuid();

            // act
            // assert
            var exception = Assert.ThrowsException<RemindersApplicationException>(() => service.Delete(id));

            Assert.IsTrue(exception.StatusCode == ValidationStatus.NotFound);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.NotFound));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_NotFindDeletedOnDelete()
        {
            // arrange
            repository.Add(new Reminder("Title", "Description", DateTime.UtcNow.AddDays(1), false));

            var id = repository.Get().FirstOrDefault().Id;
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            service.Delete(id);

            // assert
            var exception = Assert.ThrowsException<RemindersApplicationException>(() => service.Delete(id));

            Assert.IsTrue(exception.StatusCode == ValidationStatus.NotFound);
            Assert.IsTrue(exception.Message.Contains(RemindersResources.NotFound));
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_GetInactive()
        {
            // arrange
            repository.Add(new Reminder("Title", "Description", DateTime.UtcNow.AddDays(1), false));

            var id = repository.Get().FirstOrDefault().Id;
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            service.Delete(id);

            var result = service.GetInactive(id);

            // assert
            Assert.IsTrue(repository.Get(id).IsDeleted);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_GetInactiveItems()
        {
            // arrange
            repository.Add(new Reminder("Title", "Description", DateTime.UtcNow.AddDays(1), false));

            var id = repository.Get().FirstOrDefault().Id;
            var service = new RemindersService(mapperMock, repository, unitOfWorkMock.Object);

            // act
            service.Delete(id);

            var result = service.GetInactive();

            // assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(repository.Get(id).IsDeleted);
        }

        [Timeout(1000)]
        [TestMethod]
        public void Should_CreateReminderIntance()
        {
            // arrange, act
            var reminder = new Reminder();

            // assert
            Assert.IsInstanceOfType(reminder, typeof(Reminder));
        }
    }
}