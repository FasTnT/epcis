namespace FasTnT.Model
{
    public class EpcisQueryCallbackDocument : Request
    {
        public EpcisEvent[] EventList { get; set; }
        public string SubscriptionName { get; set; }
    }
}
