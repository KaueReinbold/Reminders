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

namespace Reminders.Mvc.Test.Screen
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
            string dateLimit = DateTime.Now.AddDays(10).ToString("ddMMyyyy");
            
            _webDriver.SetText(By.Id("Title"), $"{_enumBrowsers.ToString()} | Title - {newGuid}");
            _webDriver.SetText(By.Id("Description"), $"Description - {newGuid}");
            _webDriver.SetText(By.Id("LimitDate"), dateLimit);

            _webDriver.Submit(By.Id("formCreate"));

            var texts = _webDriver.GetTexts(By.CssSelector("#remindersTable > tbody > tr > td:nth-child(2)"));

            Assert.IsFalse(texts.Contains(newGuid), $"{_enumBrowsers.ToString()} - The insert has not happened!");
        }
    }
}
