using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;
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
            await StoreStandardBusinessHeader(request, headerId);
            await StoreCustomFields(request, headerId);

            // TODO: store events and masterdata.
        }

        private async Task<int> PersistHeader(CaptureDocumentRequest request)
        {
            var parameters = new { request.Payload.Header.DocumentTime, request.Payload.Header.RecordTime, UserId = request.User.Id };
            var commandDefinition = new CommandDefinition(
                commandText: CaptureEpcisDocumentCommands.PersistHeader,
                parameters: parameters,
                transaction: request.Transaction,
                cancellationToken: request.CancellationToken
            );

            return await _connection.QuerySingleAsync<int>(commandDefinition);
        }

        private async Task StoreCustomFields(CaptureDocumentRequest request, int headerId)
        {
            var customFields = request.Payload.Header.CustomFields;

            if (customFields == null || !customFields.Any()) return;

            var parameters = customFields.Select(c => new
            {
                HeaderId = headerId,
                c.Children,
                c.DateValue,
                c.Name,
                c.Namespace,
                c.NumericValue,
                c.TextValue,
                c.Type
            });

            await _connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistHeader, parameters, request.Transaction, cancellationToken: request.CancellationToken);
        }

        private async Task StoreStandardBusinessHeader(CaptureDocumentRequest request, int headerId)
        {
            var contactInformations = request.Payload.Header.StandardBusinessHeader?.ContactInformations;

            if (contactInformations == null || !contactInformations.Any()) return;

            var parameters = contactInformations.Select(ci => new
            {
                HeaderId = headerId,
                ci.Contact,
                ci.EmailAddress,
                ci.Identifier,
                ci.TelephoneNumber,
                ci.Type,
                ci.FaxNumber,
                ci.ContactTypeIdentifier
            });

            await _connection.BulkInsertAsync("", parameters, request.Transaction, cancellationToken: request.CancellationToken);
        }
    }
}
