using System.Xml.Linq;
using FasTnT.Model.Subscriptions;

namespace FasTnT.Formatters.Xml
{
    public static class XmlSubscriptionParser
    {
        public static SubscriptionRequest ParseSubscription(XElement element)
        {
            // TODO: parse subscription request
            return new Subscription();
        }

        public static SubscriptionRequest ParseUnsubscription(XElement element)
        {
            // TODO: parse unsubscription request
            return new UnsubscribeRequest();
        }
    }
}
