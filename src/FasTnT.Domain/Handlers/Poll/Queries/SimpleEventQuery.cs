using FasTnT.Commands.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Queries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Handlers.Poll.Queries
{
    public static class SimpleEventQuery
    {
        public static async Task<IEpcisResponse> Handle(QueryParameter[] parameters)
        {
            return new PollResponse
            {
                QueryName = "SimpleEventQuery",
                EventList = new[]
                {
                    new EpcisEvent
                    {
                        Type = EventType.Object,
                        Action = EventAction.Add,
                        BusinessLocation = "TestLocation",
                        Epcs = new []{ new Epc{ Type = EpcType.List, Id = "url:epc:123456.789" } }.ToList(),
                        EventTime = DateTime.UtcNow,
                        EventTimeZoneOffset = TimeZoneOffset.Default,
                        CaptureTime = DateTime.UtcNow
                    }
                }.ToList()
            };
            //throw new EpcisException(ExceptionType.ImplementationException, "SimpleEventQuery is not implemented yet");
        }
    }
}
