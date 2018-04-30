using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Reminders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Reminders.Domain.Extensions;

namespace Reminders.Api.Test.Reminders
{
    [TestClass]
    public class RemindersApiTests : StartupApiTest
    {
        private string _baseUri;
        private string _testGuid;

        public RemindersApiTests()
        {
            _baseUri = _configuration["TestConfiguration:UrlApi"] + "api/reminders";

            _testGuid = Guid.NewGuid().ToString();
        }

        [TestMethod]
        public void RemindersApiCRUD()
        {
            RemindersInsert();

            RemindersEdit();

            RemindersDelete();
        }

        public void RemindersInsert()
        {
            var success = false;

            var reminder = new ReminderModel
            {
                Title = $"Api Test - Title - {_testGuid}",
                Description = $"Api Test - Description - {_testGuid}",
                LimitDate = DateTime.UtcNow.AddDays(10),
                IsDone = false
            };

            var result = _httpClient.PostAsJsonAsync(_baseUri, reminder).Result;

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var reminders = _httpClient.GetAsync(_baseUri).Result.Content.ReadAsJsonAsync<IList<ReminderModel>>().Result;

                success = reminders.Any(r => r.Title.Contains(_testGuid));
            }

            if (!success)
                Assert.Fail("The insert has not happened!");
        }

        public void RemindersEdit()
        {
            var success = false;

            var reminders = _httpClient.GetAsync(_baseUri).Result.Content.ReadAsJsonAsync<IList<ReminderModel>>().Result;

            if (reminders.Any())
            {
                var reminder = reminders.FirstOrDefault(r => r.Title.Contains(_testGuid));

                if (reminder != null)
                {
                    reminder.Title = $"Api Test - Title Edited - {_testGuid}";
                    reminder.Description = $"Api Test - Description - {_testGuid}";
                    reminder.LimitDate = DateTime.UtcNow.AddDays(5);
                    reminder.IsDone = true;

                    var result = _httpClient.PutAsJsonAsync(_baseUri, reminder).Result;

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        reminders = _httpClient.GetAsync(_baseUri).Result.Content.ReadAsJsonAsync<IList<ReminderModel>>().Result;

                        success = reminders.Any(r => r.Title.Contains(_testGuid));
                    }
                }
            }

            if (!success)
                Assert.Fail("The update has not happened!");
        }

        public void RemindersDelete()
        {
            var success = false;
            
            var reminders = _httpClient.GetAsync(_baseUri).Result.Content.ReadAsJsonAsync<IList<ReminderModel>>().Result;

            if (reminders.Any())
            {
                foreach (var reminder in reminders)
                {
                    if (reminder.Title.Contains(_testGuid))
                    {
                        success = _httpClient.DeleteAsync(_baseUri + "/" + reminder.Id).Result.StatusCode == HttpStatusCode.OK;

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
