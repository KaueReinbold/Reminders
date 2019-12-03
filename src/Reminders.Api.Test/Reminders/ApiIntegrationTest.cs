using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Reminders.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Reminders.Api.Test.Reminders.Default
{
    [TestClass]
    public class ApiIntegrationTest
        : StartupApiTest
    {
        private readonly string baseUri;
        private readonly string testGuid;

        public ApiIntegrationTest()
        {
            baseUri = Configuration["ApiBaseUrl"] + "/Reminders";

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
            var success = false;

            var reminder = new ReminderViewModel
            {
                Title = $"{testGuid}",
                Description = $"{testGuid}",
                LimitDate = DateTime.UtcNow.AddDays(10),
                IsDone = false
            };

            var request = new HttpRequestMessage(HttpMethod.Post, baseUri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(reminder), Encoding.UTF8, "application/json")
            };

            var result = httpClient.SendAsync(request).Result;

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var resultString = httpClient.GetAsync(baseUri).Result.Content.ReadAsStringAsync().Result;

                var reminders = JsonConvert.DeserializeObject<IList<ReminderViewModel>>(resultString);

                success = reminders.Any(r => r.Title.Contains(testGuid));
            }

            if (!success)
                Assert.Fail("Insert has not happened!");
        }

        public void Edit()
        {
            var success = false;

            var resultString = httpClient.GetAsync(baseUri).Result.Content.ReadAsStringAsync().Result;

            var reminders = JsonConvert.DeserializeObject<IList<ReminderViewModel>>(resultString);

            if (reminders.Any(r => r.Title.Contains(testGuid)))
            {
                var reminder = reminders.FirstOrDefault(r => r.Title.Contains(testGuid));
                reminder.Title = $"{testGuid}";
                reminder.Description = $"{testGuid}";
                reminder.LimitDate = DateTime.UtcNow.AddDays(5);
                reminder.IsDone = true;

                var request = new HttpRequestMessage(HttpMethod.Put, baseUri + "/" + testGuid)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(reminder), Encoding.UTF8, "application/json")
                };

                var result = httpClient.SendAsync(request).Result;

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    resultString = httpClient.GetAsync(baseUri).Result.Content.ReadAsStringAsync().Result;

                    reminders = JsonConvert.DeserializeObject<IList<ReminderViewModel>>(resultString);

                    success = reminders.Any(r => r.Title.Contains(testGuid));
                }
            }

            if (!success)
                Assert.Fail("Update has not happened!");
        }

        public void Delete()
        {
            var result = httpClient.DeleteAsync(baseUri + "/" + testGuid).Result;

            if (result.StatusCode != HttpStatusCode.OK)
                Assert.Fail("Delete has not happened!");
        }
    }
}
