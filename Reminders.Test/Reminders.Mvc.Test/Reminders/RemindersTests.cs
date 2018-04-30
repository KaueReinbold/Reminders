using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;
using Reminders.Mvc.Test.Selenium;
using Reminders.Mvc.Test.Selenium.Enums;

namespace Reminders.Mvc.Test.Reminders
{
    public class RemindersTests : StartupScreenTest
    {

        public RemindersTests(EnumBrowsers enumBrowsers) 
            : base(enumBrowsers)
        {

        }

        public void RemindersInsert()
        {
            _webDriver.LoadPage(TimeSpan.FromSeconds(secondsToWait), $"{_configuration.GetSection("TestConfiguration:UrlApplicationMvc").Value}Reminders/Create");

            string newGuid = Guid.NewGuid().ToString();
            DateTime dateLimit = DateTime.Now.AddDays(10);

            _webDriver.SetText(By.Id("Title"), $"{_enumBrowsers.ToString()} | Title - {newGuid}");
            _webDriver.SetText(By.Id("Description"), $"Description - {newGuid}");
            _webDriver.SetTextJavascript("#LimitDate", dateLimit.ToString("yyyy-MM-dd"));

            _webDriver.Submit("#formCreate");

            _webDriver.WaitForElement(By.Id("remindersTable"), secondsToWait);

            var texts = _webDriver.GetTexts(By.CssSelector("#remindersTable > tbody > tr > td:nth-child(2)"));

            var hasGuid = texts.Any(text => text.Contains(newGuid));

            Assert.IsTrue(hasGuid, $"{_enumBrowsers.ToString()} - The insert has not happened!");
        }

        public void RemindersEdit()
        {
            _webDriver.LoadPage(TimeSpan.FromSeconds(secondsToWait), $"{_configuration.GetSection("TestConfiguration:UrlApplicationMvc").Value}");

            _webDriver.WaitForElement(By.Id("remindersTable"), secondsToWait);

            var lines = _webDriver.FindElements(By.CssSelector("#remindersTable > tbody > tr"));

            var textsOld = lines.Where(line => line.FindElement(By.CssSelector("td:nth-child(2)")).Text.StartsWith(_enumBrowsers.ToString()));

            var link = textsOld.Select(line => line.FindElement(By.CssSelector("td:nth-child(6) > a:nth-child(1)")).GetAttribute("href"))?.FirstOrDefault();

            _webDriver.LoadPage(TimeSpan.FromSeconds(secondsToWait), link);

            string newGuid = Guid.NewGuid().ToString();

            _webDriver.FindElement(By.Id("Title")).Clear();
            _webDriver.SetText(By.Id("Title"), $"{_enumBrowsers.ToString()} | Title Edited - {newGuid}");

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
            _webDriver.LoadPage(TimeSpan.FromSeconds(secondsToWait), $"{_configuration.GetSection("TestConfiguration:UrlApplicationMvc").Value}");

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
