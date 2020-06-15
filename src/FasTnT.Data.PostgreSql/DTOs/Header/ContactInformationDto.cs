using FasTnT.Model.Enums;
using FasTnT.Model.Headers;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class ContactInformationDto
    {
        public int Id { get; set; }
        public int HeaderId { get; set; }
        public ContactInformationType Type { get; set; }
        public string Identifier { get; set; }
        public string Contact { get; set; }
        public string EmailAddress { get; set; }
        public string FaxNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string ContactTypeIdentifier { get; set; }

        public static ContactInformationDto Create(ContactInformation contact, int requestId, int index)
        {
            return new ContactInformationDto
            {
                Id = index,
                HeaderId = requestId,
                Type = contact.Type,
                Identifier = contact.Identifier,
                Contact = contact.Contact,
                EmailAddress = contact.EmailAddress,
                FaxNumber = contact.FaxNumber,
                TelephoneNumber = contact.TelephoneNumber,
                ContactTypeIdentifier = contact.ContactTypeIdentifier
            };
        }
    }
}
