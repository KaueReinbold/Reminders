using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reminders.Api.Controllers;
using Reminders.Application.Contracts;
using Reminders.Application.ViewModels;

namespace Reminders.Application.Test.Controllers
{
    [TestClass]
    public class RemindersControllerUnitTest
    {
        #region Variables

        private Mock<IRemindersService> mockRemindersService;
        private RemindersController controller;

        #endregion

        #region Initialize

        [TestInitialize]
        public void TestInitialize()
        {
            mockRemindersService = new Mock<IRemindersService>();
            controller = new RemindersController(mockRemindersService.Object);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Should_CreateController_WithValidService()
        {
            // arrange & act
            var controller = new RemindersController(mockRemindersService.Object);

            // assert
            Assert.IsNotNull(controller);
        }

        #endregion

        #region GET Tests

        [TestMethod]
        public void Should_Get_ReturnOkResult_WithReminders()
        {
            // arrange
            var expectedReminders = new List<ReminderViewModel>
            {
                new ReminderViewModel { Id = Guid.NewGuid(), Title = "Test 1", Description = "Description 1" },
                new ReminderViewModel { Id = Guid.NewGuid(), Title = "Test 2", Description = "Description 2" }
            };

            mockRemindersService
                .Setup(s => s.Get())
                .Returns(expectedReminders.AsQueryable());

            // act
            var result = controller.Get();

            // assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var reminders = okResult.Value as IQueryable<ReminderViewModel>;
            Assert.IsNotNull(reminders);
            Assert.AreEqual(2, reminders.Count());
            
            mockRemindersService.Verify(s => s.Get(), Times.Once);
        }

        [TestMethod]
        public void Should_Get_ReturnOkResult_WithEmptyList()
        {
            // arrange
            mockRemindersService
                .Setup(s => s.Get())
                .Returns(new List<ReminderViewModel>().AsQueryable());

            // act
            var result = controller.Get();

            // assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var reminders = okResult.Value as IQueryable<ReminderViewModel>;
            Assert.IsNotNull(reminders);
            Assert.AreEqual(0, reminders.Count());
        }

        #endregion

        #region GET Count Tests

        [TestMethod]
        public void Should_Count_ReturnOkResult_WithCorrectCount()
        {
            // arrange
            var reminders = new List<ReminderViewModel>
            {
                new ReminderViewModel { Id = Guid.NewGuid(), Title = "Test 1" },
                new ReminderViewModel { Id = Guid.NewGuid(), Title = "Test 2" },
                new ReminderViewModel { Id = Guid.NewGuid(), Title = "Test 3" }
            };

            mockRemindersService
                .Setup(s => s.Get())
                .Returns(reminders.AsQueryable());

            // act
            var result = controller.Count();

            // assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(3, okResult.Value);
        }

        [TestMethod]
        public void Should_Count_ReturnZero_WhenNoReminders()
        {
            // arrange
            mockRemindersService
                .Setup(s => s.Get())
                .Returns(new List<ReminderViewModel>().AsQueryable());

            // act
            var result = controller.Count();

            // assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(0, okResult.Value);
        }

        #endregion

        #region GET By Id Tests

        [TestMethod]
        public async Task Should_GetById_ReturnOkResult_WithReminder()
        {
            // arrange
            var reminderId = Guid.NewGuid();
            var expectedReminder = new ReminderViewModel 
            { 
                Id = reminderId, 
                Title = "Test Reminder", 
                Description = "Test Description" 
            };

            mockRemindersService
                .Setup(s => s.GetAsync(reminderId))
                .ReturnsAsync(expectedReminder);

            // act
            var result = await controller.Get(reminderId);

            // assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var reminder = okResult.Value as ReminderViewModel;
            Assert.IsNotNull(reminder);
            Assert.AreEqual(reminderId, reminder.Id);
            Assert.AreEqual("Test Reminder", reminder.Title);
            
            mockRemindersService.Verify(s => s.GetAsync(reminderId), Times.Once);
        }

        [TestMethod]
        public async Task Should_GetById_ReturnOkResult_WithNull_WhenNotFound()
        {
            // arrange
            var reminderId = Guid.NewGuid();

            mockRemindersService
                .Setup(s => s.GetAsync(reminderId))
                .ReturnsAsync((ReminderViewModel)null);

            // act
            var result = await controller.Get(reminderId);

            // assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNull(okResult.Value);
        }

        #endregion

        #region POST Tests

        [TestMethod]
        public async Task Should_Post_ReturnCreatedReminder()
        {
            // arrange
            var inputReminder = new ReminderViewModel
            {
                Title = "New Reminder",
                Description = "New Description",
                LimitDate = DateTime.UtcNow.AddDays(1)
            };

            var createdReminder = new ReminderViewModel
            {
                Id = Guid.NewGuid(),
                Title = inputReminder.Title,
                Description = inputReminder.Description,
                LimitDate = inputReminder.LimitDate
            };

            mockRemindersService
                .Setup(s => s.InsertAsync(inputReminder))
                .ReturnsAsync(createdReminder);

            // act
            var result = await controller.Post(inputReminder);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(createdReminder.Id, result.Id);
            Assert.AreEqual(createdReminder.Title, result.Title);
            
            mockRemindersService.Verify(s => s.InsertAsync(inputReminder), Times.Once);
        }

        #endregion

        #region PUT Tests

        [TestMethod]
        public async Task Should_Put_ReturnUpdatedReminder()
        {
            // arrange
            var reminderId = Guid.NewGuid();
            var inputReminder = new ReminderViewModel
            {
                Id = reminderId,
                Title = "Updated Reminder",
                Description = "Updated Description",
                LimitDate = DateTime.UtcNow.AddDays(2)
            };

            var updatedReminder = new ReminderViewModel
            {
                Id = reminderId,
                Title = inputReminder.Title,
                Description = inputReminder.Description,
                LimitDate = inputReminder.LimitDate
            };

            mockRemindersService
                .Setup(s => s.EditAsync(reminderId, inputReminder))
                .ReturnsAsync(updatedReminder);

            // act
            var result = await controller.Put(reminderId, inputReminder);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(reminderId, result.Id);
            Assert.AreEqual("Updated Reminder", result.Title);
            
            mockRemindersService.Verify(s => s.EditAsync(reminderId, inputReminder), Times.Once);
        }

        #endregion

        #region DELETE Tests

        [TestMethod]
        public async Task Should_Delete_CallServiceDelete()
        {
            // arrange
            var reminderId = Guid.NewGuid();

            mockRemindersService
                .Setup(s => s.DeleteAsync(reminderId))
                .Returns(Task.CompletedTask);

            // act
            await controller.Delete(reminderId);

            // assert
            mockRemindersService.Verify(s => s.DeleteAsync(reminderId), Times.Once);
        }

        #endregion
    }
}