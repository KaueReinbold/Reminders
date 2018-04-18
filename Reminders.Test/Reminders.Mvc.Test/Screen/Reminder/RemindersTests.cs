using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.IO;
using Reminders.Mvc.Test.Selenium;
using Reminders.Mvc.Test.Selenium.Enums;
using OpenQA.Selenium.Edge;

namespace Reminders.Mvc.Test.Screen.Reminder
{
    public class RemindersTests
    {
        private readonly IConfiguration _configuration;
        private IWebDriver _webDriver;
        private EnumBrowsers _enumBrowsers;

        public RemindersTests(EnumBrowsers enumBrowsers)
        {
            _enumBrowsers = enumBrowsers;

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile($"appsettings.json");

            _configuration = builder.Build();

            string path = $"{Environment.CurrentDirectory}\\";

            switch (enumBrowsers)
            {
                case EnumBrowsers.Firefox:
                    path = string.Concat(path, _configuration.GetSection("Selenium:FirefoxDriver").Value);
                    break;
                case EnumBrowsers.Chrome:
                    path = string.Concat(path, _configuration.GetSection("Selenium:ChromeDriver").Value);
                    break;
                case EnumBrowsers.Edge:
                    path = string.Concat(path, _configuration.GetSection("Selenium:EdgeDriver").Value);
                    break;
            }

            _webDriver = WebDriverFactory.CreateWebDriver(enumBrowsers, path);
        }

        public void Close()
        {
            _webDriver.Quit();
            _webDriver = null;
        }

        public void RemindersInsert()
        {
            _webDriver.LoadPage(TimeSpan.FromSeconds(5), $"{_configuration.GetSection("TestConfigutation:UrlApplicationMvc").Value}Reminders/Create");

            string newGuid = Guid.NewGuid().ToString();
            DateTime dateLimit = DateTime.Now.AddDays(10);

            _webDriver.SetText(By.Id("Title"), $"{_enumBrowsers.ToString()} | Title - {newGuid}");
            _webDriver.SetText(By.Id("Description"), $"Description - {newGuid}");

            if (_webDriver is EdgeDriver)
                _webDriver.SetTextJavascript("#LimitDate", dateLimit.ToString("yyyy-MM-dd"));
            else
                _webDriver.SetText(By.Id("LimitDate"), dateLimit.ToString("dd/MM/yyyy"));

            _webDriver.Submit("#formCreate");

            var texts = _webDriver.GetTexts(By.CssSelector("#remindersTable > tbody > tr > td:nth-child(2)"));

            var hasGuid = texts.Any(text => text.Contains(newGuid));

            Assert.IsTrue(hasGuid, $"{_enumBrowsers.ToString()} - The insert has not happened!");
        }

        public void RemindersDelete()
        {
            _webDriver.LoadPage(TimeSpan.FromSeconds(5), $"{_configuration.GetSection("TestConfigutation:UrlApplicationMvc").Value}");

            var lines = _webDriver.FindElements(By.CssSelector("#remindersTable > tbody > tr"));

            var textsOld = lines.Where(line => line.FindElement(By.CssSelector("td:nth-child(2)")).Text.Contains(_enumBrowsers.ToString()));

             var links = textsOld.Select(line => line.FindElement(By.CssSelector("td:nth-child(6) > a:nth-child(3)")).GetAttribute("href")).ToList();

            links.ForEach(link =>
            {
                _webDriver.LoadPage(TimeSpan.FromSeconds(5), link);

                _webDriver.Submit("#formDelete");
            });

            var textsNew = _webDriver.GetTexts(By.CssSelector("#remindersTable > tbody > tr > td:nth-child(2)"));

            var hasBrowser = textsNew.Any(text => text.Contains(_enumBrowsers.ToString()));

            Assert.IsFalse(hasBrowser, $"{_enumBrowsers.ToString()} - The delete has not happened!");
        }
    }
}
