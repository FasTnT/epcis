using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Model.Subscriptions;

namespace FasTnT.Formatters.Xml
{
    public class XmlSubscriptionFormatter : ISubscriptionFormatter
    {
        public SubscriptionRequest Read(Stream input)
        {
            var document = XDocument.Load(input);

            if (document.Root.Name.LocalName == "EPCISQueryDocument")
            {
                var element = document.Root.Element("EPCISBody").Elements().FirstOrDefault();

                if (element != null)
                {
                    if (element.Name == XName.Get("Subscribe", EpcisNamespaces.Query))
                    {
                        return XmlSubscriptionParser.ParseSubscription(element);
                    }
                    if(element.Name == XName.Get("Unsubscribe", EpcisNamespaces.Query))
                    {
                        return XmlSubscriptionParser.ParseUnsubscription(element);
                    }
                }
            }

            throw new Exception($"Element not expected: '{document.Root.Name.LocalName}'");
        }

        public void Write(SubscriptionRequest entity, Stream output)
        {
            throw new NotImplementedException();
        }
    }
}
