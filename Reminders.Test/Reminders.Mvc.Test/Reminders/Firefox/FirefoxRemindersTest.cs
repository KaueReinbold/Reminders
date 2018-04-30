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
        public void FirefoxReminderCRUD()
        {
            _remindersTests.RemindersInsert();

            _remindersTests.RemindersEdit();

            _remindersTests.RemindersDelete();
        }
    }
}
