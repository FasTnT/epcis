using System;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class StandardBusinessHeader
    {
        public string Version { get; set; }
        public IList<ContactInformation> Senders { get; set; } = new List<ContactInformation>();
        public IList<ContactInformation> Receivers { get; set; } = new List<ContactInformation>();
        public string Standard { get; set; }
        public string TypeVersion { get; set; }
        public string InstanceIdentifier { get; set; }
        public string Type { get; set; }
        public DateTime? CreationDateTime { get; set; }
    }
}
