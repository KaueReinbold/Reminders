using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Mvc.Test.Screen.Selenium.Enums;

namespace Reminders.Mvc.Test.Screen.Reminder.Chrome
{
    [TestClass]
    public class ChromeRemindersTest
    {
        private RemindersTests _remindersTests;

        [TestInitialize]
        public void TestInitialize()
        {
            _remindersTests = new RemindersTests(EnumBrowsers.Chrome);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _remindersTests.Close();
        }

        [TestMethod]
        public void ChromeReminderInsert()
        {
            _remindersTests.RemindersInsert();
        }

        [TestMethod]
        public void ChromeReminderEdit()
        {
            _remindersTests.RemindersEdit();
        }

        [TestMethod]
        public void ChromeReminderDelete()
        {
            _remindersTests.RemindersDelete();
        }
    }
}
