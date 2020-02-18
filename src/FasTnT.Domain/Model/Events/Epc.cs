using FasTnT.Model.Events.Enums;

namespace FasTnT.Model
{
    public class Epc
    {
        public int? EventId { get; set; }
        public string Id { get; set; }
        public EpcType Type { get; set; }
        public bool IsQuantity { get; set; }
        public float? Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
    }
}
