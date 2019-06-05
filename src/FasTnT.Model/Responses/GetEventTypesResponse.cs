using System.Collections.Generic;

namespace FasTnT.Model.Responses
{
    public class GetEventTypesResponse : IEpcisResponse
    {
        public IEnumerable<string> EventTypes { get; set; }
    }
}
