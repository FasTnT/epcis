using FasTnT.Commands.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Parsers
{
    [TestClass]
    public class WhenParsingXmlSubscribeRequest : XmlParserTestBase
    {
        public override void Given()
        {
            SetRequest("<?xml version=\"1.0\" encoding=\"utf-8\"?><epcisq:EPCISQueryDocument xmlns:epcisq=\"urn:epcglobal:epcis-query:xsd:1\" creationDate=\"2019-01-26T20:10:01.8111457Z\" schemaVersion=\"1\"><EPCISBody><epcisq:Subscribe><queryName>SimpleEventQuery</queryName><params /><dest>http://callback/url</dest><controls><schedule><second>0</second></schedule><reportIfEmpty>true</reportIfEmpty></controls><subscriptionID>TestSubscriptionId</subscriptionID></epcisq:Subscribe></EPCISBody></epcisq:EPCISQueryDocument>");
        }

        [TestMethod]
        public void ItShouldReturnASubscribeRequest()
        {
            Assert.IsInstanceOfType(Result, typeof(SubscribeRequest));
        }

        [TestMethod]
        public void ItShouldHaveTheSpecifiedSchedule()
        {
            var subscribe = (SubscribeRequest)Result;

            Assert.AreEqual("0", subscribe.Subscription.Schedule.Second);
            Assert.IsNull(subscribe.Subscription.Schedule.DayOfMonth);
            Assert.IsNull(subscribe.Subscription.Schedule.DayOfWeek);
            Assert.IsNull(subscribe.Subscription.Schedule.Hour);
            Assert.IsNull(subscribe.Subscription.Schedule.Minute);
            Assert.IsNull(subscribe.Subscription.Schedule.Month);
        }

        [TestMethod]
        public void ItShouldHaveTheSpecifiedDestination()
        {
            var subscribe = (SubscribeRequest)Result;

            Assert.AreEqual("http://callback/url", subscribe.Subscription.Destination);
        }

        [TestMethod]
        public void ItShouldHaveTheSpecifiedSubscriptionId()
        {
            var subscribe = (SubscribeRequest)Result;

            Assert.AreEqual("TestSubscriptionId", subscribe.Subscription.SubscriptionId);
        }
    }
}
