using FasTnT.Model.Utils;

namespace FasTnT.Model.Events.Enums
{
    public class ContactInformationType : Enumeration
    {
        public static ContactInformationType Sender = new ContactInformationType(0, "Sender");
        public static ContactInformationType Receiver = new ContactInformationType(1, "Receiver");

        public ContactInformationType() { }
        public ContactInformationType(short id, string displayName) : base(id, displayName) { }
    }
}
