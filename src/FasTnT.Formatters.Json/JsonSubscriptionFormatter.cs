using System.IO;
using FasTnT.Model.Subscriptions;

namespace FasTnT.Formatters.Json
{
    public class JsonSubscriptionFormatter : ISubscriptionFormatter
    {
        public SubscriptionRequest Read(Stream input)
        {
            throw new System.NotImplementedException();
        }

        public void Write(SubscriptionRequest entity, Stream output)
        {
            throw new System.NotImplementedException();
        }
    }
}
