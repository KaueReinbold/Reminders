using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using Reminders.Infrastructure.CrossCutting.Configuration;
using Reminders.Mvc.Test.Selenium.Enums;
using System;
using System.IO;

namespace Reminders.Mvc.Test.Selenium
{
    public static class WebDriverFactory
    {
        private static IConfiguration _configuration;

        public static IWebDriver CreateWebDriver(EnumBrowsers enumBrowsers, PlatformType platformType)
        {
            _configuration = IConfigurationHelper.GetConfiguration();

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

            IWebDriver webDriver = null;

            switch (enumBrowsers)
            {
                case EnumBrowsers.Firefox:
                    var firefoxOptions = new FirefoxOptions();
                    firefoxOptions.AddArgument("--headless");
                    firefoxOptions.AddArgument("--lang=pt");
                    //SetPlatform(firefoxOptions, platformType);
                    webDriver = new FirefoxDriver(path, firefoxOptions);
                    break;
                case EnumBrowsers.Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--headless");
                    chromeOptions.AddArgument("--lang=pt");
                    //SetPlatform(chromeOptions, platformType);
                    webDriver = new ChromeDriver(path, chromeOptions);
                    break;
                case EnumBrowsers.Edge:
                    var edgeOptions = new EdgeOptions();
                    //SetPlatform(edgeOptions, platformType);
                    // BUG: Edge does not support headless mode. Check for updates. 
                    webDriver = new EdgeDriver(path, edgeOptions);
                    break;
            }

            return webDriver;
        }

        private static T SetPlatform<T>(T options, PlatformType platformType) 
            where T : DriverOptions
        {
            switch (platformType)
            {
                case PlatformType.Any:
                    return options;

                case PlatformType.Windows:
                    options.PlatformName = "WINDOWS";
                    return options;

                case PlatformType.Linux:
                    options.PlatformName = "LINUX";
                    return options;

                case PlatformType.Mac:
                    options.PlatformName = "MAC";
                    return options;

                default:
                    return options;
            }
        }
    }
}
