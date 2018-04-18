using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Reminders.Mvc.Test.Screen
{
    [TestClass]
    public class ScreenBaseTest
    {
        public IWebDriver _webDriver;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _webDriver = new ChromeDriver($"{Environment.CurrentDirectory}/Drivers");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _webDriver.Quit();
        }
    }
}
