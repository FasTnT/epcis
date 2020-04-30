using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Model;
using MoreLinq;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.Capture
{
    public static class HeaderStore
    {
        public static async Task<int> Store(EpcisRequest request, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var parameters = new { request.DocumentTime, request.RecordTime, request.UserId };
            var headerId = await transaction.Connection.ExecuteScalarAsync<int>(new CommandDefinition(CaptureEpcisDocumentCommands.PersistHeader, parameters, transaction, cancellationToken: cancellationToken));

            await StoreStandardBusinessHeader(request, headerId, transaction, cancellationToken);
            await StoreCustomFields(request, headerId, transaction, cancellationToken);

            return headerId;
        }

        private static async Task StoreCustomFields(EpcisRequest header, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var customFields = header.CustomFields;

            if (customFields == null || !customFields.Any()) return;
            customFields.ForEach(x => x.HeaderId = headerId);

            await transaction.Connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistHeader, customFields, transaction, cancellationToken: cancellationToken);
        }

        private static async Task StoreStandardBusinessHeader(EpcisRequest header, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
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
