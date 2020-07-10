using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Subscriptions.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FasTnT.UnitTest.Subscriptions
{
    [TestClass]
    public class SubscriptionScheduleTests
    {
        [TestMethod]
        public void WhenRetrievingTheNextOccurenceTime()
        {
            var schedule = new SubscriptionSchedule(new QuerySchedule { Hour = "1" });
            var nextOccurence = schedule.GetNextOccurence(new DateTime(2019, 08, 01, 15, 59, 05));

            Assert.AreEqual(new DateTime(2019, 08, 02, 01, 00, 00), nextOccurence);
        }

        [TestMethod]
        public void WhenRetrievingTheNextOccurenceTimeWhenCurrentDateIsOccurenceTime()
        {
            var schedule = new SubscriptionSchedule(new QuerySchedule { Hour = "1", Minute = "0", Second = "0" });
            var nextOccurence = schedule.GetNextOccurence(new DateTime(2019, 08, 01, 01, 00, 00));

            Assert.AreEqual(new DateTime(2019, 08, 02, 01, 00, 00), nextOccurence);
        }
    }
}
