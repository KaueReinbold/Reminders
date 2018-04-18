using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using Reminders.Mvc.Test.Selenium.Enums;

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
                    webDriver = new FirefoxDriver(FirefoxDriverService.CreateDefaultService(path));
                    break;
                case EnumBrowsers.Chrome:
                    webDriver = new ChromeDriver(path);
                    break;
                case EnumBrowsers.Edge:
                    webDriver = new EdgeDriver(path);
                    break;
            }

            return webDriver;
        }
    }
}
