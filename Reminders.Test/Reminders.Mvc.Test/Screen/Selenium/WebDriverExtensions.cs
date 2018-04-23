using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reminders.Mvc.Test.Screen.Selenium
{
    public static class WebDriverExtensions
    {
        public static void LoadPage(this IWebDriver webDriver, TimeSpan timeToWait, string url)
        {
            webDriver.Manage().Timeouts().PageLoad = timeToWait;
            webDriver.Navigate().GoToUrl(url);
        }

        public static string GetText(this IWebDriver webDriver, By by)
        {
            IWebElement webElement = webDriver.FindElement(by);

            return webElement.Text;
        }

        public static List<string> GetTexts(this IWebDriver webDriver, By by)
        {
            ICollection<IWebElement> webElements = webDriver.FindElements(by);

            return webElements.Select(element => element.Text).ToList();
        }

        public static void SetText(this IWebDriver webDriver, By by, string text)
        {
            IWebElement webElement = webDriver.FindElement(by);

            webElement.SendKeys(text);
        }

        public static void SetTextJavascript(this IWebDriver webDriver, string querySelector, string text)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            string javascriptCode = $"document.querySelector('{querySelector}').value = '{text}';";
            js.ExecuteScript(javascriptCode);
        }

        public static void Submit(this IWebDriver webDriver, string querySelector)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            string javascriptCode = $"document.querySelector('{querySelector}').submit();";
            js.ExecuteScript(javascriptCode);
        }

        public static void WaitForElement(this IWebDriver webDriver, By by, int secondsToWait = 5)
        {
            WebDriverWait webDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(secondsToWait));

            webDriverWait.Until(c => c.FindElement(by) != null);
        }
    }
}
