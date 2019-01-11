using FasTnT.Model.Utils;

namespace FasTnT.Model.Events.Enums
{
    public class FieldType : Enumeration
    {
        public static FieldType Ilmd = new FieldType(0, "Ilmd");
        public static FieldType EventExtension = new FieldType(1, "EventExtension");
        public static FieldType ErrorDeclarationExtension = new FieldType(2, "ErrorDeclarationExtension");
        public static FieldType ReadPointExtension = new FieldType(3, "ReadPointExtension");
        public static FieldType Attribute = new FieldType(4, "Attribute");
        public static FieldType BusinessLocationExtension = new FieldType(5, "BusinessLocationExtension");

        public FieldType()
        {
        }

        public FieldType(short id, string displayName) : base(id, displayName)
        {
        }
    }
}
