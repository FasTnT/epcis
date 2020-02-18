using FasTnT.Commands.Requests;
using FasTnT.Domain.Commands;
using FasTnT.Domain.Model.Subscriptions;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Query
{
    public static class XmlSubscriptionParser
    {
        public static IQueryRequest ParseSubscription(XElement element)
        {
            var subscription = new Subscription
            {
                SubscriptionId = element.Element("subscriptionID").Value,
                QueryName = element.Element("queryName").Value,
                Destination = element.Element("dest").Value,
                Trigger = element.Element("controls")?.Element("trigger")?.Value,
                ReportIfEmpty = bool.Parse(element.Element("controls").Element("reportIfEmpty").Value),
                InitialRecordTime = DateTime.TryParse(element.Element("controls")?.Element("initialRecordTime")?.Value ?? "", out DateTime date) ? date : default(DateTime?),
                Parameters = XmlPollQueryParser.ParseParameters(element.Element("params")?.Elements()).ToArray()
            };
            subscription.Schedule = ParseQuerySchedule(element.Element("controls")?.Element("schedule"));

            return new SubscribeRequest { Subscription = subscription };
        }

        public static IQueryRequest ParseUnsubscription(XElement element)
        {
            return new UnsubscribeRequest
            {
                SubscriptionId = element.Element("subscriptionID").Value
            };
        }

        private static QuerySchedule ParseQuerySchedule(XElement element)
        {
            return element == default ? null : new QuerySchedule
            {
                Second = element.Element("second")?.Value,
                Minute = element.Element("minute")?.Value,
                Hour = element.Element("hour")?.Value,
                Month = element.Element("month")?.Value,
                DayOfMonth = element.Element("dayOfMonth")?.Value,
                DayOfWeek = element.Element("dayOfWeek")?.Value
            };
        }
    }
}
