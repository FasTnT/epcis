using System.Collections.Generic;

namespace FasTnT.Model.Responses
{
    public class GetQueryNamesResponse : IEpcisResponse
    {
        public IEnumerable<string> QueryNames { get; set; }
    }
}
