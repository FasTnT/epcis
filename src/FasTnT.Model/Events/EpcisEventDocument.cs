using System;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class EpcisEventDocument : Request
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IEnumerable<EpcisEvent> EventList { get; set; }
    }
}
