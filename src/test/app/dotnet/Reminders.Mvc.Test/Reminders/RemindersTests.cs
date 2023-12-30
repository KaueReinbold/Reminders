using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;
using Reminders.Mvc.Test.Selenium;
using Reminders.Mvc.Test.Selenium.Enums;
using Microsoft.Extensions.Configuration;
using Reminders.Infrastructure.CrossCutting.Configuration;

namespace Reminders.Mvc.Test.Reminders
{
    [TestClass]
    public class RemindersTests
    {
        public IWebDriver _webDriver;
        private IConfiguration Configuration;
        private string applicationBaseUrl;
        public EnumBrowsers _enumBrowsers;
        public readonly int secondsToWait = 15;

        [TestInitialize]
        public void TestInitialize()
        {
            // TODO: Find a way to use development appsettings.
            Configuration = IConfigurationHelper.GetConfiguration();

            applicationBaseUrl = $"{Configuration.GetSection("TestConfiguration:UrlApplicationMvc").Value}";
        }

        [TestMethod]
        [DataRow(EnumBrowsers.Chrome, PlatformType.Windows)]
        [DataRow(EnumBrowsers.Firefox, PlatformType.Windows)]
        public void RemindersCRUD(EnumBrowsers browsers, PlatformType platformType)
        {
            _enumBrowsers = browsers;
            _webDriver = WebDriverFactory.CreateWebDriver(browsers, platformType);

            RemindersInsert();

            RemindersEdit();

            RemindersDelete();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _webDriver.Close();
            _webDriver.Quit();
            _webDriver.Dispose();
        }

        public void RemindersInsert()
        {
            _webDriver.LoadPage(TimeSpan.FromSeconds(secondsToWait), $"{applicationBaseUrl}/Reminders/Create");

            string newGuid = Guid.NewGuid().ToString();
            DateTime dateLimit = DateTime.Now.AddDays(10);

            _webDriver.SetText(By.Id("Title"), $"{_enumBrowsers.ToString()} | {newGuid}");
            _webDriver.SetText(By.Id("Description"), $"Description - {newGuid}");
            _webDriver.SetTextJavascript("#LimitDate", dateLimit.ToString("yyyy-MM-ddTHH:mm"));

            _webDriver.Submit("#formCreate");

            _webDriver.WaitForElement(By.Id("remindersTable"), secondsToWait);

            var texts = _webDriver.GetTexts(By.CssSelector("#remindersTable > tbody > tr > td:nth-child(2)"));

            var hasGuid = texts.Any(text => text.Contains(newGuid));

            Assert.IsTrue(hasGuid, $"{_enumBrowsers.ToString()} - The insert has not happened!");
        }

        public void RemindersEdit()
        {
            _webDriver.LoadPage(TimeSpan.FromSeconds(secondsToWait), $"{applicationBaseUrl}");

            _webDriver.WaitForElement(By.Id("remindersTable"), secondsToWait);

            var lines = _webDriver.FindElements(By.CssSelector("#remindersTable > tbody > tr"));

            var textsOld = lines.Where(line => line.FindElement(By.CssSelector("td:nth-child(2)")).Text.StartsWith(_enumBrowsers.ToString()));

            var link = textsOld.Select(line => line.FindElement(By.CssSelector("td:nth-child(6) > a:nth-child(1)")).GetAttribute("href"))?.FirstOrDefault();

            _webDriver.LoadPage(TimeSpan.FromSeconds(secondsToWait), link);

            string newGuid = Guid.NewGuid().ToString();

            _webDriver.FindElement(By.Id("Title")).Clear();
            _webDriver.SetText(By.Id("Title"), $"{_enumBrowsers.ToString()} | {newGuid}");

            _webDriver.FindElement(By.Id("Description")).Clear();
            _webDriver.SetText(By.Id("Description"), $"Description Edited - {newGuid}");

            _webDriver.FindElement(By.Id("IsDone")).Click();

            _webDriver.Submit("#formEdit");

            _webDriver.WaitForElement(By.Id("remindersTable"), secondsToWait);

            var texts = _webDriver.GetTexts(By.CssSelector("#remindersTable > tbody > tr > td:nth-child(2)"));

            var hasGuid = texts.Any(text => text.Contains(newGuid));

            Assert.IsTrue(hasGuid, $"{_enumBrowsers.ToString()} - The update has not happened!");
        }

        public void RemindersDelete()
        {
            _webDriver.LoadPage(TimeSpan.FromSeconds(secondsToWait), $"{applicationBaseUrl}");

            var lines = _webDriver.FindElements(By.CssSelector("#remindersTable > tbody > tr"));

            var textsOld = lines.Where(line => line.FindElement(By.CssSelector("td:nth-child(2)")).Text.StartsWith(_enumBrowsers.ToString()));

            var links = textsOld.Select(line => line.FindElement(By.CssSelector("td:nth-child(6) > a:nth-child(3)")).GetAttribute("href")).ToList();

            links.ForEach(link =>
            {
                _webDriver.LoadPage(TimeSpan.FromSeconds(secondsToWait), link);

                _webDriver.Submit("#formDelete");
            });

            var textsNew = _webDriver.GetTexts(By.CssSelector("#remindersTable > tbody > tr > td:nth-child(2)"));

            var hasBrowser = textsNew.Any(text => text.StartsWith(_enumBrowsers.ToString()));

            Assert.IsFalse(hasBrowser, $"{_enumBrowsers.ToString()} - The delete has not happened!");
        }
    }
}
