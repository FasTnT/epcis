using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;
using MoreLinq;
using System.Data;
using System.Linq;
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

        public async Task Capture(CaptureDocumentRequest request)
        {
            var headerId = await PersistHeader(request);

            await EpcisEventStore.StoreEpcisEvents(request, _connection, headerId);
            await EpcisMasterdataStore.StoreEpcisMasterdata(request, _connection, headerId);
        }

        private async Task<int> PersistHeader(CaptureDocumentRequest request)
        {
            var parameters = new { request.Payload.Header.DocumentTime, request.Payload.Header.RecordTime, UserId = request.User.Id };
            var headerId = await _connection.ExecuteScalarAsync<int>(new CommandDefinition(CaptureEpcisDocumentCommands.PersistHeader, parameters, request.Transaction, cancellationToken: request.CancellationToken));

            await StoreStandardBusinessHeader(request, headerId);
            await StoreCustomFields(request, headerId);

            return headerId;
        }

        private async Task StoreCustomFields(CaptureDocumentRequest request, int headerId)
        {
            var customFields = request.Payload.Header.CustomFields;

            if (customFields == null || !customFields.Any()) return;
            customFields.ForEach(x => x.HeaderId = headerId);

            await _connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistHeader, customFields, request.Transaction, cancellationToken: request.CancellationToken);
        }

        private async Task StoreStandardBusinessHeader(CaptureDocumentRequest request, int headerId)
        {
            if (request.Payload.Header.StandardBusinessHeader == null) return;

            request.Payload.Header.StandardBusinessHeader.Id = headerId;

            await _connection.ExecuteAsync(new CommandDefinition(CaptureEpcisDocumentCommands.PersistStandardHeader, request.Payload.Header.StandardBusinessHeader, request.Transaction, cancellationToken: request.CancellationToken));

            var contactInformations = request.Payload.Header.StandardBusinessHeader?.ContactInformations;

            if (contactInformations == null || !contactInformations.Any()) return;
            contactInformations.ForEach((x, i) => { x.HeaderId = headerId; x.Id = i; });

            await _connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistContactInformations, contactInformations, request.Transaction, cancellationToken: request.CancellationToken);
        }
    }
}
