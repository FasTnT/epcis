using FasTnT.Model.Utils;

namespace FasTnT.Model.Enums
{
    public class EventType : Enumeration
    {
        public static readonly EventType Object = new EventType(0, "ObjectEvent");
        public static readonly EventType Aggregation = new EventType(1, "AggregationEvent");
        public static readonly EventType Transaction = new EventType(2, "TransactionEvent");
        public static readonly EventType Transformation = new EventType(3, "TransformationEvent");
        public static readonly EventType Quantity = new EventType(4, "QuantityEvent");

        public EventType() { }
        public EventType(short id, string displayName) : base(id, displayName) { }
    }
}
