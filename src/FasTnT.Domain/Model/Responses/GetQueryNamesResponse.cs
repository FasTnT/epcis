using FasTnT.Domain.Formatter;
using System.Collections.Generic;

namespace FasTnT.Domain.Model.Responses
{
    public class GetQueryNamesResponse : IEpcisResponse
    {
        public IEnumerable<string> QueryNames { get; set; }

        public void Accept(IEpcisResponseFormatter formatter) => formatter.Accept(this);
    }
}
