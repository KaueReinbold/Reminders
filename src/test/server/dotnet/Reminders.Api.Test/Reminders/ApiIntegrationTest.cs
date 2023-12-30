using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Application.ViewModels;
using Reminders.Api.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Reminders.Api.Test.Reminders.Default
{
    [TestClass]
    public class ApiIntegrationTest
        : StartupApiTest
    {
        private string baseUri;
        private string testGuid;

        [TestInitialize]
        public void TestInitialize()
        {
            baseUri = string.Concat(BaseAddress, "/Reminders");

            testGuid = Guid.NewGuid().ToString();
        }

        [TestMethod]
        public void Should_Perform_CRUD_For_Reminder()
        {
            Insert();

            Edit();

            Delete();
        }

        public void Insert()
        {
            // Arrange
            var success = false;

            var reminder = new ReminderViewModel
            {
                Title = $"{testGuid}",
                Description = $"{testGuid}",
                LimitDate = DateTime.UtcNow.AddDays(10),
                IsDone = false
            };

            // Act
            var result = httpClient.PostAsync(new Uri(baseUri), reminder.ToStringContent()).Result;

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var resultString = httpClient.GetAsync(baseUri).Result.Content.ReadAsStringAsync().Result;

                var reminders = resultString.FromJson<IList<ReminderViewModel>>();

                success = reminders.Any(r => r.Title.Contains(testGuid));
            }

            // Assert
            Assert.IsTrue(success, "Insert has not happened!");
        }

        public void Edit()
        {
            // Arrange
            var success = false;

            var resultString = httpClient.GetAsync(new Uri(baseUri)).Result.Content.ReadAsStringAsync().Result;

            var reminders = resultString.FromJson<IList<ReminderViewModel>>();

            // Act
            if (reminders.Any(r => r.Title.Contains(testGuid)))
            {
                var reminder = reminders.FirstOrDefault(r => r.Title.Contains(testGuid));

                reminder.Title = $"{testGuid}";
                reminder.Description = $"{testGuid}";
                reminder.LimitDate = DateTime.UtcNow.AddDays(5);
                reminder.IsDone = true;

                var result = httpClient.PutAsync(new Uri(baseUri + "/" + reminder.Id), reminder.ToStringContent()).Result;

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    resultString = httpClient.GetAsync(baseUri).Result.Content.ReadAsStringAsync().Result;

                    reminders = resultString.FromJson<IList<ReminderViewModel>>();

                    success = reminders.Any(r => r.Title.Contains(testGuid));
                }
            }

            // Assert
            Assert.IsTrue(success, "Update has not happened!");
        }

        public void Delete()
        {
            // Arrange
            var success = false;

            var resultString = httpClient.GetAsync(new Uri(baseUri)).Result.Content.ReadAsStringAsync().Result;

            var reminders = resultString.FromJson<IList<ReminderViewModel>>();

            // Act
            if (reminders.Any(r => r.Title.Contains(testGuid)))
            {
                var reminder = reminders.FirstOrDefault(r => r.Title.Contains(testGuid));

                var result = httpClient.DeleteAsync(new Uri(baseUri + "/" + reminder.Id)).Result;

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    resultString = httpClient.GetAsync(baseUri).Result.Content.ReadAsStringAsync().Result;

                    reminders = resultString.FromJson<IList<ReminderViewModel>>();

                    success = !reminders.Any(r => r.Title.Contains(testGuid));
                }
            }

            // Assert
            Assert.IsTrue(success, "Delete has not happened!");
        }
    }
}
