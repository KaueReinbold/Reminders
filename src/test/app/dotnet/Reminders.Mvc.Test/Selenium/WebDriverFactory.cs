using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Reminders.Mvc.Test.Selenium.Enums;

namespace Reminders.Mvc.Test.Selenium
{
    // Drivers are resolved by Selenium Manager (bundled with Selenium 4.6+),
    // so no driver binaries or paths are needed on any OS.
    public static class WebDriverFactory
    {
        public static IWebDriver CreateWebDriver(EnumBrowsers enumBrowsers)
        {
            switch (enumBrowsers)
            {
                case EnumBrowsers.Firefox:
                    var firefoxOptions = new FirefoxOptions();
                    firefoxOptions.AddArgument("--headless");
                    firefoxOptions.AddArgument("--width=1366");
                    firefoxOptions.AddArgument("--height=1000");
                    firefoxOptions.AcceptInsecureCertificates = true;
                    return new FirefoxDriver(firefoxOptions);

                case EnumBrowsers.Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--headless=new");
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-gpu");
                    chromeOptions.AddArgument("--window-size=1366,1000");
                    return new ChromeDriver(chromeOptions);

                default:
                    throw new ArgumentOutOfRangeException(nameof(enumBrowsers), enumBrowsers, "Unsupported browser");
            }
        }
    }
}
