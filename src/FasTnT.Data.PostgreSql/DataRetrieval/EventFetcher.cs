using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Data.PostgreSql.DataRetrieval
{
    public class EventFetcher : IEventFetcher
    {
        public void Apply(SimpleParameterFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task<EpcisEvent[]> Fetch(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
