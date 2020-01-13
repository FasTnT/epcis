using FasTnT.Model.Events.Enums;

namespace FasTnT.Model.Events
{
    public class ContactInformation
    {
        public int? Id { get; set; }
        public int? HeaderId { get; set; }
        public ContactInformationType Type { get; set; }
        public string Identifier { get; set; }
        public string Contact { get; set; }
        public string EmailAddress { get; set; }
        public string FaxNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string ContactTypeIdentifier { get; set; }
    }
}
