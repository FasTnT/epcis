using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Data.PostgreSql.Utils;
using FasTnT.Domain.Data.Model;
using FasTnT.Model;
using MoreLinq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using StoreAction = System.Func<FasTnT.Domain.Data.Model.CaptureDocumentRequest, System.Data.IDbConnection, System.Threading.Tasks.Task>;

namespace FasTnT.PostgreSql.Capture
{
    public static class EpcisEventStore
    {
        private static readonly StoreAction[] Actions = new StoreAction[] { StoreEvents, StoreEpcs, StoreCustomFields, StoreSourceDestinations, StoreBusinessTransactions, StoreErrorDeclaration };

        public static async Task StoreEpcisEvents(CaptureDocumentRequest request, IDbConnection connection, int headerId)
        {
            if (request.Payload.EventList == null || !request.Payload.EventList.Any()) return;

            request.Payload.EventList.ForEach(e => e.RequestId = headerId);

            foreach (var action in Actions)
            {
                await action(request, connection);
            }
        }

        private async static Task StoreEvents(CaptureDocumentRequest request, IDbConnection connection)
        {
            var eventIds = await connection.BulkInsertReturningAsync(CaptureEpcisEventCommand.StoreEvent, request.Payload.EventList, request.Transaction, cancellationToken: request.CancellationToken);
            var idArray = eventIds.ToArray();

            for (var i = 0; i < request.Payload.EventList.Count; i++)
            {
                request.Payload.EventList[i].Id = idArray[i];
            }
        }

        private async static Task StoreEpcs(CaptureDocumentRequest request, IDbConnection connection)
        {
            var epcs = request.Payload.EventList.SelectMany(e => { e.Epcs.ForEach(x => x.EventId = e.Id); return e.Epcs; });
            await connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreEpcs, epcs, request.Transaction, cancellationToken: request.CancellationToken);
        }

        private async static Task StoreCustomFields(CaptureDocumentRequest request, IDbConnection connection)
        {
            var fields = new List<CustomField>();
            request.Payload.EventList.ForEach(evt => PgSqlCustomFieldsParser.ParseFields(evt.CustomFields, evt.Id.Value, fields));

            await connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreCustomField, fields, request.Transaction, cancellationToken: request.CancellationToken);
        }

        private async static Task StoreSourceDestinations(CaptureDocumentRequest request, IDbConnection connection)
        {
            var sourceDest = request.Payload.EventList.SelectMany(e => { e.SourceDestinationList.ForEach(sd => sd.EventId = e.Id); return e.SourceDestinationList; });
            await connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreSourceDestination, sourceDest, request.Transaction, cancellationToken: request.CancellationToken);
        }

        private async static Task StoreBusinessTransactions(CaptureDocumentRequest request, IDbConnection connection)
        {
            var tx = request.Payload.EventList.SelectMany(e => { e.BusinessTransactions.ForEach(x => x.EventId = e.Id); return e.BusinessTransactions; });
            await connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreBusinessTransaction, tx, request.Transaction, cancellationToken: request.CancellationToken);
        }

        private async static Task StoreErrorDeclaration(CaptureDocumentRequest request, IDbConnection connection)
        {
            var eventsWithErrorDeclaration = request.Payload.EventList.Where(x => x.ErrorDeclaration != null);

            var declarations = eventsWithErrorDeclaration.Select(e => { e.ErrorDeclaration.EventId = e.Id; return e; });
            var corrective = eventsWithErrorDeclaration.SelectMany(x => { x.ErrorDeclaration.CorrectiveEventIds.ForEach(r => r.EventId = x.Id); return x.ErrorDeclaration.CorrectiveEventIds; });

            await connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreErrorDeclaration, declarations, request.Transaction, cancellationToken: request.CancellationToken);
            await connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreErrorDeclarationIds, corrective, request.Transaction, cancellationToken: request.CancellationToken);
        }
    }
}
