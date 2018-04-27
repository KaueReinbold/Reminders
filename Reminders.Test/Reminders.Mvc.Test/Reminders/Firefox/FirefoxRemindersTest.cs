using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Mvc.Test.Selenium.Enums;

namespace Reminders.Mvc.Test.Reminders.Firefox
{
    [TestClass]
    public class FirefoxRemindersTest
    {
        private RemindersTests _remindersTests;

        [TestInitialize]
        public void TestInitialize()
        {
            _remindersTests = new RemindersTests(EnumBrowsers.Firefox);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _remindersTests.Close();
        }

        [TestMethod]
        public void FirefoxReminderInsert()
        {
            _remindersTests.RemindersInsert();
        }

        [TestMethod]
        public void FirefoxReminderEdit()
        {
            _remindersTests.RemindersEdit();
        }

        [TestMethod]
        public void FirefoxReminderDelete()
        {
            _remindersTests.RemindersDelete();
        }
    }
}
