using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Mvc.Test.Selenium.Enums;

namespace Reminders.Mvc.Test.Reminders.Chrome
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
        public void ChromeRemindersCRUD()
        {
            _remindersTests.RemindersInsert();

            _remindersTests.RemindersEdit();

            _remindersTests.RemindersDelete();
        }
    }
}
