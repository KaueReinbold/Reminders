using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using Reminders.Mvc.Test.Selenium.Enums;
using System;

namespace Reminders.Mvc.Test.Selenium
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
                    firefoxOptions.AddArgument("--headless");
                    firefoxOptions.AddArgument("--lang=pt");
                    webDriver = new FirefoxDriver(path, firefoxOptions);
                    break;
                case EnumBrowsers.Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--headless");
                    chromeOptions.AddArgument("--lang=pt");
                    webDriver = new ChromeDriver(path, chromeOptions);
                    break;
                case EnumBrowsers.Edge:
                    var edgeOptions = new EdgeOptions();
                    webDriver = new EdgeDriver(path, edgeOptions);
                    break;
            }

            return webDriver;
        }
    }
}
