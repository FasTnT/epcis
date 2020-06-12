using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class EventDto
    {
        public int RequestId { get; set; }
        public short Id { get; set; }
        public DateTime EventTime { get; set; }
        public DateTime CaptureTime { get; set; }
        public EventAction Action { get; set; }
        public EventType Type { get; set; }
        public TimeZoneOffset EventTimeZoneOffset { get; set; }
        public string BusinessLocation { get; set; }
        public string BusinessStep { get; set; }
        public string Disposition { get; set; }
        public string ReadPoint { get; set; }
        public string TransformationId { get; set; }
        public string EventId { get; set; }
        public DateTime? ErrorDeclarationTime { get; set; }
        public string ErrorDeclarationReason { get; set; }

        internal EpcisEvent ToEpcisEvent()
        {
            return new EpcisEvent
            {
                Action = Action,
                BusinessLocation = BusinessLocation,
                BusinessStep = BusinessStep,
                CaptureTime = CaptureTime,
                Disposition = Disposition,
                EventId = EventId,
                EventTime = EventTime,
                ReadPoint = ReadPoint,
                EventTimeZoneOffset = EventTimeZoneOffset,
                TransformationId = TransformationId,
                Type = Type,
                CorrectiveDeclarationTime = ErrorDeclarationTime,
                CorrectiveReason = ErrorDeclarationReason
            };
        }

        internal static EventDto Create(EpcisEvent epcisEvent, short eventId, int requestId)
        {
            return new EventDto
            {
                Id = eventId,
                RequestId = requestId,
                EventTime = epcisEvent.EventTime,
                ReadPoint = epcisEvent.ReadPoint,
                EventTimeZoneOffset = epcisEvent.EventTimeZoneOffset,
                TransformationId = epcisEvent.TransformationId,
                BusinessLocation = epcisEvent.BusinessLocation,
                BusinessStep = epcisEvent.BusinessStep,
                Disposition = epcisEvent.Disposition,
                EventId = epcisEvent.EventId,
                Action = epcisEvent.Action,
                Type = epcisEvent.Type,
                ErrorDeclarationTime = epcisEvent.CorrectiveDeclarationTime,
                ErrorDeclarationReason = epcisEvent.CorrectiveReason
            };
        }
    }
}
