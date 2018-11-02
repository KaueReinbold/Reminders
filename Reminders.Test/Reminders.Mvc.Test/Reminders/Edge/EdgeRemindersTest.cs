using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Mvc.Test.Selenium.Enums;

namespace Reminders.Mvc.Test.Reminders.Edge
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
        public void EdgeRemindersCRUD()
        {
            _remindersTests.RemindersInsert();

            _remindersTests.RemindersEdit();

            _remindersTests.RemindersDelete();
        }
    }
}
