using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Mvc.Test.Screen.Selenium.Enums;

namespace Reminders.Mvc.Test.Screen.Reminder.Edge
{
    [TestClass]
    public class EdgeRemindersTest
    {
        private RemindersTests _remindersTests;

        [TestInitialize]
        public void TestInitialize()
        {
            _remindersTests = new RemindersTests(EnumBrowsers.Edge);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _remindersTests.Close();
        }

        [TestMethod]
        public void EdgeReminderInsert()
        {
            _remindersTests.RemindersInsert();
        }

        [TestMethod]
        public void EdgeReminderEdit()
        {
            _remindersTests.RemindersEdit();
        }

        [TestMethod]
        public void EdgeReminderDelete()
        {
            _remindersTests.RemindersDelete();
        }
    }
}
