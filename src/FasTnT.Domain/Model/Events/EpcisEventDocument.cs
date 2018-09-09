using System;
using System.Collections.Generic;

namespace FasTnT.Domain
{
    public class EpcisEventDocument : EpcisCapture
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IEnumerable<EpcisEvent> EventList { get; set; }
    }
}
