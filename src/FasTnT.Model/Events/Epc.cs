using FasTnT.Model.Events.Enums;
using System;

namespace FasTnT.Model
{
    public class Epc
    {
        public string Id { get; set; }
        public EpcType Type { get; set; }
        public bool IsQuantity { get; set; }
        public float? Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
    }
}
