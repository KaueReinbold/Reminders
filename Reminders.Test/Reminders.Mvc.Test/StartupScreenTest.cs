using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using Reminders.Core.Options;
using Reminders.Mvc.Test.Selenium;
using Reminders.Mvc.Test.Selenium.Enums;
using System;
using System.IO;

namespace Reminders.Mvc.Test
{
    public class StartupScreenTest
    {
        public readonly IConfiguration _configuration;
        public IWebDriver _webDriver;
        public readonly EnumBrowsers _enumBrowsers;
        public readonly int secondsToWait = 10;

        public StartupScreenTest(EnumBrowsers enumBrowsers)
        {
            _enumBrowsers = enumBrowsers;

            // Add the appsettings file.
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            string path = $"{Environment.CurrentDirectory}\\";

            switch (enumBrowsers)
            {
                case EnumBrowsers.Firefox:
                    path = string.Concat(path, _configuration.GetSection("TestConfiguration:PathFirefoxDriver").Value);
                    break;
                case EnumBrowsers.Chrome:
                    path = string.Concat(path, _configuration.GetSection("TestConfiguration:PathChromeDriver").Value);
                    break;
                case EnumBrowsers.Edge:
                    path = string.Concat(path, _configuration.GetSection("TestConfiguration:PathEdgeDriver").Value);
                    break;
            }

            _webDriver = WebDriverFactory.CreateWebDriver(enumBrowsers, path);
        }

        public void Close()
        {
            _webDriver.Quit();
            _webDriver = null;
        }
    }
}
