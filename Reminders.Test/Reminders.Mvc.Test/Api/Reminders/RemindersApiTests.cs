using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Reminders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Reminders.Domain.Extensions;

namespace Reminders.Mvc.Test.Api.Reminders
{
    [TestClass]
    public class RemindersApiTests : TestConfigurationApi
    {
        [TestMethod]
        public void RemindersInsert()
        {
            var baseUri = _configuration["TestConfigutation:UrlApi"] + "api/reminders";
            var success = false;

            var newGuid = Guid.NewGuid().ToString();

            var reminder = new ReminderModel
            {
                Title = $"Api Test - Title - {newGuid}",
                Description = $"Api Test - Description - {newGuid}",
                LimitDate = DateTime.UtcNow.AddDays(10),
                IsDone = false
            };

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var result = _httpClient.PostAsJsonAsync(baseUri, reminder).Result;

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var reminders = _httpClient.GetAsync(baseUri).Result.Content.ReadAsJsonAsync<IList<ReminderModel>>().Result;

                success = reminders.Any(r => r.Title.Contains(newGuid));
            }

            if (!success)
                Assert.Fail("The insert has not happened!");
        }

        [TestMethod]
        public void RemindersEdit()
        {
            var baseUri = _configuration["TestConfigutation:UrlApi"] + "api/reminders";
            var success = false;

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var reminders = _httpClient.GetAsync(baseUri).Result.Content.ReadAsJsonAsync<IList<ReminderModel>>().Result;

            if (reminders.Any())
            {
                var reminder = reminders.FirstOrDefault(r => r.Title.StartsWith("Api Test"));

                if (reminder != null)
                {
                    var newGuid = Guid.NewGuid().ToString();
                    reminder.Title = $"Api Test - Title Edited - {newGuid}";
                    reminder.Description = $"Api Test - Description - {newGuid}";
                    reminder.LimitDate = DateTime.UtcNow.AddDays(5);
                    reminder.IsDone = true;

                    var result = _httpClient.PutAsJsonAsync(baseUri, reminder).Result;

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        reminders = _httpClient.GetAsync(baseUri).Result.Content.ReadAsJsonAsync<IList<ReminderModel>>().Result;

                        success = reminders.Any(r => r.Title.Contains(newGuid));
                    }
                }
            }

            if (!success)
                Assert.Fail("The update has not happened!");
        }

        [TestMethod]
        public void RemindersDelete()
        {
            var baseUri = _configuration["TestConfigutation:UrlApi"] + "api/reminders";
            var success = false;

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var reminders = _httpClient.GetAsync(baseUri).Result.Content.ReadAsJsonAsync<IList<ReminderModel>>().Result;

            if (reminders.Any())
            {
                foreach (var reminder in reminders)
                {
                    if (reminder.Title.StartsWith("Api Test"))
                    {
                        success = _httpClient.DeleteAsync(baseUri + "/" + reminder.Id).Result.StatusCode == HttpStatusCode.OK;

                        if (!success)
                            break;
                    }
                }
            }

            if (!success)
                Assert.Fail("The delete has not happened!");
        }
    }
}
