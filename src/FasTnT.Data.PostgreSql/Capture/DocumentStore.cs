using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Domain;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;
using MoreLinq;
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


        public async Task Capture(CaptureDocumentRequest request, RequestContext context, CancellationToken cancellationToken)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var headerId = await PersistHeader(request, transaction, context, cancellationToken);

                await EpcisEventStore.StoreEpcisEvents(request, transaction, headerId, cancellationToken);
                await EpcisMasterdataStore.StoreEpcisMasterdata(request, transaction, headerId, cancellationToken);

                transaction.Commit();
            }
        }

        private async Task<int> PersistHeader(CaptureDocumentRequest request, IDbTransaction transaction, RequestContext context, CancellationToken cancellationToken)
        {
            var parameters = new { request.Header.DocumentTime, request.Header.RecordTime, UserId = context.User.Id };
            var headerId = await transaction.Connection.ExecuteScalarAsync<int>(new CommandDefinition(CaptureEpcisDocumentCommands.PersistHeader, parameters, transaction, cancellationToken: cancellationToken));

            await StoreStandardBusinessHeader(request, headerId, transaction, cancellationToken);
            await StoreCustomFields(request, headerId, transaction, cancellationToken);

            return headerId;
        }

        private async Task StoreCustomFields(CaptureDocumentRequest request, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var customFields = request.Header.CustomFields;

            if (customFields == null || !customFields.Any()) return;
            customFields.ForEach(x => x.HeaderId = headerId);

            await transaction.Connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistHeader, customFields, transaction, cancellationToken: cancellationToken);
        }

        private async Task StoreStandardBusinessHeader(CaptureDocumentRequest request, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            if (request.Header.StandardBusinessHeader == null) return;

            request.Header.StandardBusinessHeader.Id = headerId;

            await transaction.Connection.ExecuteAsync(new CommandDefinition(CaptureEpcisDocumentCommands.PersistStandardHeader, request.Header.StandardBusinessHeader, transaction, cancellationToken: cancellationToken));

            var contactInformations = request.Header.StandardBusinessHeader?.ContactInformations;

            if (contactInformations == null || !contactInformations.Any()) return;
            contactInformations.ForEach((x, i) => { x.HeaderId = headerId; x.Id = i; });

            await transaction.Connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistContactInformations, contactInformations, transaction, cancellationToken: cancellationToken);
        }
    }
}
