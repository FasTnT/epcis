using System;
using System.Collections.Generic;

namespace FasTnT.Model.Headers
{
    public class StandardBusinessHeader
    {
        public int? Id { get; set; }
        public string Version { get; set; }
        public IList<ContactInformation> ContactInformations { get; set; } = new List<ContactInformation>();
        public string Standard { get; set; }
        public string TypeVersion { get; set; }
        public string InstanceIdentifier { get; set; }
        public string Type { get; set; }
        public DateTime? CreationDateTime { get; set; }
    }
}
