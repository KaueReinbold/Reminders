using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using Reminders.Mvc.Test.Screen.Selenium.Enums;
using System;

namespace Reminders.Mvc.Test.Screen.Selenium
{
    public static class WebDriverFactory
    {
        public static IWebDriver CreateWebDriver(EnumBrowsers enumBrowsers, string path)
        {
            IWebDriver webDriver = null;

            switch (enumBrowsers)
            {
                case EnumBrowsers.Firefox:
                    var firefoxOptions = new FirefoxOptions();
                    firefoxOptions.AddArguments("--lang=pt");
                    webDriver = new FirefoxDriver(FirefoxDriverService.CreateDefaultService(path), firefoxOptions, TimeSpan.FromSeconds(10));
                    break;
                case EnumBrowsers.Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArguments("--lang=pt");
                    webDriver = new ChromeDriver(path, chromeOptions);
                    break;
                case EnumBrowsers.Edge:
                    webDriver = new EdgeDriver(path);
                    break;
            }

            return webDriver;
        }
    }
}
