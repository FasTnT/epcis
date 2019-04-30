using System;
using FasTnT.Model.Events;

namespace FasTnT.Persistence.Dapper
{
    public class ContactInformationEntity : ContactInformation
    {
        public int Id { get; set; }
        public Guid HeaderId { get; set; }
    }
}
