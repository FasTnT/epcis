using FasTnT.Domain;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;
using FasTnT.Model;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.Capture
{
    public class EpcisRequestStore : IEpcisRequestStore
    {
        private readonly IDbConnection _connection;

        public EpcisRequestStore(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task Capture(EpcisRequest request, RequestContext context, CancellationToken cancellationToken)
        {
            using (var tx = _connection.BeginTransaction())
            {
                var headerId = await HeaderStore.Store(request, tx, cancellationToken);
                await SubscriptionCallbackStore.Store(request, headerId, tx, cancellationToken);
                await EpcisEventStore.StoreEpcisEvents(request.EventList, tx, headerId, cancellationToken);
                await EpcisMasterdataStore.StoreEpcisMasterdata(request.MasterdataList, tx, cancellationToken);

                tx.Commit();
            };
        }
    }
}
