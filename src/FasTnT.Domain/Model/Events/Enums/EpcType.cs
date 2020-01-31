using FasTnT.Model.Utils;

namespace FasTnT.Model.Events.Enums
{
    public class EpcType : Enumeration
    {
        public static EpcType List = new EpcType(0, "list");
        public static EpcType ParentId = new EpcType(1, "parentID");
        public static EpcType InputQuantity = new EpcType(2, "inputQuantity");
        public static EpcType OutputQuantity = new EpcType(3, "outputQuantity");
        public static EpcType InputEpc = new EpcType(4, "inputEPC");
        public static EpcType OutputEpc = new EpcType(5, "outputEPC");
        public static EpcType Quantity = new EpcType(6, "quantity");
        public static EpcType ChildEpc = new EpcType(7, "childEPC");
        public static EpcType ChildQuantity = new EpcType(8, "childQuantity");

        public EpcType() {}
        public EpcType(short id, string displayName) : base(id, displayName) {}
    }
}
