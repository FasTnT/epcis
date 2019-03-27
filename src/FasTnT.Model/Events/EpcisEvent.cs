using FasTnT.Model.Events.Enums;
using FasTnT.Model.Responses;
using System;
using System.Collections.Generic;

namespace FasTnT.Model
{
    public class EpcisEvent : IEntity
    {
        public DateTime CaptureTime { get; set; }
        public DateTime EventTime { get; set; }
        public TimeZoneOffset EventTimeZoneOffset { get; set; } = TimeZoneOffset.Default;
        public EventType Type { get; set; }
        public EventAction Action { get; set; }
        public string EventId { get; set; }
        public string ReadPoint { get; set; }
        public string BusinessLocation { get; set; }
        public string BusinessStep { get; set; }
        public string Disposition { get; set; }
        public string TransformationId { get; set; }
        public ErrorDeclaration ErrorDeclaration { get; set; }
        public IList<Epc> Epcs { get; set; } = new List<Epc>();
        public IList<BusinessTransaction> BusinessTransactions { get; set; } = new List<BusinessTransaction>();
        public IList<SourceDestination> SourceDestinationList { get; set; } = new List<SourceDestination>();
        public IList<CustomField> CustomFields { get; set; } = new List<CustomField>();
    }
}
