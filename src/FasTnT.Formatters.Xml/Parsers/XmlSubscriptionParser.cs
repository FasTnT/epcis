using System.Xml.Linq;
using FasTnT.Model.Subscriptions;

namespace FasTnT.Formatters.Xml
{
    public static class XmlSubscriptionParser
    {
        public static SubscriptionRequest ParseSubscription(XElement element)
        {
            // TODO: parse subscription request
            return new Subscription
            {
                SubscriptionId = element.Element("subscriptionID").Value,
                QueryName = element.Element("queryName").Value,
                Destination = element.Element("dest").Value
            };
        }

        public static SubscriptionRequest ParseUnsubscription(XElement element)
        {
            return new UnsubscribeRequest
            {
                SubscriptionId = element.Element("subscriptionID").Value
            };
        }
    }
}
