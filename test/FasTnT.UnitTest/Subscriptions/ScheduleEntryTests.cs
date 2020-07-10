using FasTnT.Subscriptions.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FasTnT.UnitTest.Subscriptions
{
    [TestClass]
    public class ScheduleEntryTests
    {
        [TestMethod]
        public void WhenParsingAValidScheduleEntry()
        {
            var parsed = ScheduleEntry.Parse("12", 0, 24);

            Assert.IsNotNull(parsed);
            Assert.IsTrue(parsed.HasValue(12), "The value 12 should be in the parsed schedule");
            Assert.IsFalse(parsed.HasValue(18), "The value 18 should not be in the parsed schedule");
        }

        [TestMethod]
        public void WhenParsingAValidRangeScheduleEntry()
        {
            var parsed = ScheduleEntry.Parse("[12-20]", 0, 24);

            Assert.IsNotNull(parsed);
            Assert.IsTrue(parsed.HasValue(12), "The value 12 should be in the parsed schedule");
            Assert.IsTrue(parsed.HasValue(18), "The value 18 should be in the parsed schedule");
            Assert.IsFalse(parsed.HasValue(8), "The value 8 should not be in the parsed schedule");
        }

        [TestMethod]
        public void WhenParsingAnInvalidRangeScheduleEntry()
        {
            ScheduleEntry parsed = default;
            Exception catched = default;

            try
            {
                parsed = ScheduleEntry.Parse("[12, 20]", 0, 14);
            }
            catch(Exception ex)
            {
                catched = ex;
            }

            Assert.IsNull(parsed);
            Assert.IsNotNull(catched);
            Assert.IsInstanceOfType(catched, typeof(ArgumentException));
        }
    }
}
