using System;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Formatters.Xml.Requests;
using FasTnT.Model.Subscriptions;

namespace FasTnT.Formatters.Xml
{
    public static class XmlSubscriptionParser
    {
        public static SubscriptionRequest ParseSubscription(XElement element)
        {
            var subscription = new Subscription
            {
                SubscriptionId = element.Element("subscriptionID").Value,
                QueryName = element.Element("queryName").Value,
                Destination = element.Element("dest").Value,
                Trigger = element.Element("controls")?.Element("trigger")?.Value,
                ReportIfEmpty = bool.Parse(element.Element("controls").Element("reportIfEmpty").Value),
                InitialRecordTime = DateTime.TryParse(element.Element("controls")?.Element("initialRecordTime")?.Value ?? "", out DateTime date) ? date : default(DateTime?), 
                Parameters = XmlQueryParser.ParseParameters(element.Element("params")?.Elements()).ToArray()
            };
            subscription.Schedule = ParseQuerySchedule(element.Element("controls")?.Element("schedule"));

            return subscription;
        }

        public static SubscriptionRequest ParseUnsubscription(XElement element)
        {
            return new UnsubscribeRequest
            {
                SubscriptionId = element.Element("subscriptionID").Value
            };
        }

        private static QuerySchedule ParseQuerySchedule(XElement element)
        {
            return new QuerySchedule
            {
                Second = element?.Element("second")?.Value,
                Minute = element?.Element("minute")?.Value,
                Hour = element?.Element("hour")?.Value,
                Month = element?.Element("month")?.Value,
                DayOfMonth = element?.Element("dayOfMonth")?.Value,
                DayOfWeek = element?.Element("dayOfWeek")?.Value
            };
        }
    }
}
