using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Reminders.Mvc.Test.Screen.Selenium;
using Reminders.Mvc.Test.Screen.Selenium.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reminders.Mvc.Test.Screen
{
    public class TestConfigurationScreen
    {
        public readonly IConfiguration _configuration;
        public IWebDriver _webDriver;
        public readonly EnumBrowsers _enumBrowsers;
        public readonly int secondsToWait = 10;

        public TestConfigurationScreen(EnumBrowsers enumBrowsers)
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
    }
}
