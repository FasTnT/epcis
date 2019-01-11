using FasTnT.Model.Utils;

namespace FasTnT.Model.Events.Enums
{
    public class EventType : Enumeration
    {
        public static EventType Object = new EventType(0, "ObjectEvent");
        public static EventType Aggregation = new EventType(1, "AggregationEvent");
        public static EventType Transaction = new EventType(2, "TransactionEvent");
        public static EventType Transformation = new EventType(3, "TransformationEvent");
        public static EventType Quantity = new EventType(4, "QuantityEvent");

        public EventType()
        {
        }

        public EventType(short id, string displayName) : base(id, displayName)
        {
        }
    }
}
