using FasTnT.Model.Headers;
using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class StandardHeaderDto
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string Standrd { get; set; }
        public string TypeVersion { get; set; }
        public string InstanceIdentifier { get; set; }
        public string Type { get; set; }
        public DateTime? CreationDateTime { get; set; }

        internal static StandardHeaderDto Create(StandardBusinessHeader header, int requestId)
        {
            return new StandardHeaderDto
            {
                Id = requestId,
                Version = header.Version,
                Standrd = header.Standard,
                TypeVersion = header.TypeVersion,
                Type = header.Type,
                CreationDateTime = header.CreationDateTime,
                InstanceIdentifier = header.InstanceIdentifier
            };
        }
    }
}
