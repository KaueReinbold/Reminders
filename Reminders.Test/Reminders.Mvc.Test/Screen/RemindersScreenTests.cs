using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Reminders.Mvc.Test.Screen
{
    [TestClass]
    public class RemindersScreenTests : ScreenBaseTest
    {
        [TestMethod]
        public void Reminders_Insert_001()
        {
            _webDriver.Navigate().GoToUrl("http://localhost:50511/Reminders/Create");

            var newGuid = Guid.NewGuid().ToString();

            _webDriver.FindElement(By.Id("Title")).SendKeys($"Title - {newGuid}");
            _webDriver.FindElement(By.Id("Description")).SendKeys($"Description - {newGuid}");
            _webDriver.FindElement(By.Id("LimitDate")).SendKeys(DateTime.Now.AddDays(10).ToString("MMddyyy"));

            _webDriver.FindElement(By.Id("formCreate")).Submit();

            var table = _webDriver.FindElements(By.CssSelector("#remindersTable > tbody > tr"));
            bool containsGuid = false;

            table.ToList().ForEach(line =>
            {
                if (!containsGuid)
                    containsGuid = line.FindElement(By.CssSelector("td:nth-child(2)")).Text.Contains(newGuid);
            });

            if (!containsGuid)
                Assert.Fail("The insert has not happened!");
        }
    }
}
