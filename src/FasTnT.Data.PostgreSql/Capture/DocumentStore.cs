using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Domain;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;
using FasTnT.Model;
using MoreLinq;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.Capture
{
    public class DocumentStore : IDocumentStore
    {
        private readonly IDbConnection _connection;

        public DocumentStore(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task Capture(EpcisRequest request, RequestContext context, CancellationToken cancellationToken)
        {
            await Commit(async tx => 
            {
                var headerId = await PersistHeader(request, tx, context, cancellationToken);

                await EpcisEventStore.StoreEpcisEvents(request.EventList, tx, headerId, cancellationToken);
                await EpcisMasterdataStore.StoreEpcisMasterdata(request.MasterdataList, tx, headerId, cancellationToken);
            });
        }

        public async Task Capture(CaptureCallbackRequest request, RequestContext context, CancellationToken cancellationToken)
        {
            await Commit(async tx => 
            {
                var headerId = await PersistHeader(request.Header, tx, context, cancellationToken);

                await SubscriptionCallbackStore.Store(request, headerId, tx, cancellationToken);
                await EpcisEventStore.StoreEpcisEvents(request.EventList, tx, headerId, cancellationToken);
            });
        }

        private async Task Commit(Func<IDbTransaction, Task> action)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                await action(transaction);
                transaction.Commit();
            }
        }

        private async Task<int> PersistHeader(EpcisRequest header, IDbTransaction transaction, RequestContext context, CancellationToken cancellationToken)
        {
            var parameters = new { header.DocumentTime, header.RecordTime, UserId = context.User.Id };
            var headerId = await transaction.Connection.ExecuteScalarAsync<int>(new CommandDefinition(CaptureEpcisDocumentCommands.PersistHeader, parameters, transaction, cancellationToken: cancellationToken));

            await StoreStandardBusinessHeader(header, headerId, transaction, cancellationToken);
            await StoreCustomFields(header, headerId, transaction, cancellationToken);

            return headerId;
        }

        private async Task StoreCustomFields(EpcisRequest header, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var customFields = header.CustomFields;

            if (customFields == null || !customFields.Any()) return;
            customFields.ForEach(x => x.HeaderId = headerId);

            await transaction.Connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistHeader, customFields, transaction, cancellationToken: cancellationToken);
        }

        private async Task StoreStandardBusinessHeader(EpcisRequest header, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            if (header.StandardBusinessHeader == null) return;

            header.StandardBusinessHeader.Id = headerId;

            await transaction.Connection.ExecuteAsync(new CommandDefinition(CaptureEpcisDocumentCommands.PersistStandardHeader, header.StandardBusinessHeader, transaction, cancellationToken: cancellationToken));

            var contactInformations = header.StandardBusinessHeader?.ContactInformations;

            if (contactInformations == null || !contactInformations.Any()) return;
            contactInformations.ForEach((x, i) => { x.HeaderId = headerId; x.Id = i; });

            await transaction.Connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistContactInformations, contactInformations, transaction, cancellationToken: cancellationToken);
        }
    }
}
