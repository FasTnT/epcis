using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.Model.Utils;
using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class EventDto
    {
        public int RequestId { get; set; }
        public short Id { get; set; }
        public DateTime EventTime { get; set; }
        public DateTime CaptureTime { get; set; }
        public short? Action { get; set; }
        public short Type { get; set; }
        public short EventTimeZoneOffset { get; set; }
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
                Action = Action.HasValue ? Enumeration.GetById<EventAction>(Action.Value) : null,
                BusinessLocation = BusinessLocation,
                BusinessStep = BusinessStep,
                CaptureTime = CaptureTime,
                Disposition = Disposition,
                EventId = EventId,
                EventTime = EventTime,
                ReadPoint = ReadPoint,
                EventTimeZoneOffset = new TimeZoneOffset { Value = EventTimeZoneOffset },
                TransformationId = TransformationId,
                Type = Enumeration.GetById<EventType>(Type),
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
                EventTimeZoneOffset = epcisEvent.EventTimeZoneOffset.Value,
                TransformationId = epcisEvent.TransformationId,
                BusinessLocation = epcisEvent.BusinessLocation,
                BusinessStep = epcisEvent.BusinessStep,
                Disposition = epcisEvent.Disposition,
                EventId = epcisEvent.EventId,
                Action = epcisEvent.Action?.Id,
                Type = epcisEvent.Type.Id,
                ErrorDeclarationTime = epcisEvent.CorrectiveDeclarationTime,
                ErrorDeclarationReason = epcisEvent.CorrectiveReason
            };
        }
    }
}
