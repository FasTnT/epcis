using FasTnT.Model.Utils;

namespace FasTnT.Model.Enums
{
    public class FieldType : Enumeration
    {
        public static FieldType Ilmd = new FieldType(0, "Ilmd");
        public static FieldType CustomField = new FieldType(1, "CustomField");
        public static FieldType Extension = new FieldType(2, "Extension");
        public static FieldType BaseExtension = new FieldType(3, "BaseExtension");
        public static FieldType ErrorDeclarationExtension = new FieldType(4, "ErrorDeclarationExtension");
        public static FieldType ErrorDeclarationCustomField = new FieldType(5, "ErrorDeclarationCustomField");
        public static FieldType IlmdExtension = new FieldType(6, "IlmdExtension");
        public static FieldType Attribute = new FieldType(7, "Attribute");

        public FieldType() { }
        public FieldType(short id, string displayName) : base(id, displayName) { }
    }
}
