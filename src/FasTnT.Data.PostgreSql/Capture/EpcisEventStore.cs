using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Data.PostgreSql.Utils;
using FasTnT.Domain.Data.Model;
using FasTnT.Model;
using MoreLinq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoreAction = System.Func<FasTnT.Domain.Data.Model.CaptureDocumentRequest, System.Data.IDbTransaction, System.Threading.CancellationToken, System.Threading.Tasks.Task>;

namespace FasTnT.PostgreSql.Capture
{
    public static class EpcisEventStore
    {
        private static readonly StoreAction[] Actions = new StoreAction[] { StoreEvents, StoreEpcs, StoreCustomFields, StoreSourceDestinations, StoreBusinessTransactions, StoreErrorDeclaration };

        public static async Task StoreEpcisEvents(CaptureDocumentRequest request, IDbTransaction transaction, int headerId, CancellationToken cancellationToken)
        {
            if (request.EventList == null || !request.EventList.Any()) return;

            request.EventList.ForEach(e => e.RequestId = headerId);

            foreach (var action in Actions)
            {
                await action(request, transaction, cancellationToken);
            }
        }

        private async static Task StoreEvents(CaptureDocumentRequest request, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var eventIds = await transaction.Connection.BulkInsertReturningAsync(CaptureEpcisEventCommand.StoreEvent, request.EventList, transaction, cancellationToken: cancellationToken);
            var idArray = eventIds.ToArray();

            for (var i = 0; i < request.EventList.Count; i++)
            {
                request.EventList[i].Id = idArray[i];
            }
        }

        private async static Task StoreCustomFields(CaptureDocumentRequest request, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var fields = new List<CustomField>();
            request.EventList.ForEach(evt => PgSqlCustomFieldsParser.ParseFields(evt.CustomFields, evt.Id.Value, fields));

            await transaction.Connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreCustomField, fields, transaction, cancellationToken: cancellationToken);
        }

        private async static Task StoreEpcs(CaptureDocumentRequest request, IDbTransaction transaction, CancellationToken cancellationToken) => await Store(transaction, request.EventList.SelectMany(e => { e.Epcs.ForEach(x => x.EventId = e.Id); return e.Epcs; }), CaptureEpcisEventCommand.StoreEpcs, cancellationToken);
        private async static Task StoreSourceDestinations(CaptureDocumentRequest request, IDbTransaction transaction, CancellationToken cancellationToken) => await Store(transaction, request.EventList.SelectMany(e => { e.SourceDestinationList.ForEach(sd => sd.EventId = e.Id); return e.SourceDestinationList; }), CaptureEpcisEventCommand.StoreSourceDestination, cancellationToken);
        private async static Task StoreBusinessTransactions(CaptureDocumentRequest request, IDbTransaction transaction, CancellationToken cancellationToken) => await Store(transaction, request.EventList.SelectMany(e => { e.BusinessTransactions.ForEach(x => x.EventId = e.Id); return e.BusinessTransactions; }), CaptureEpcisEventCommand.StoreBusinessTransaction, cancellationToken);

        private async static Task Store<T>(IDbTransaction transaction, IEnumerable<T> elements, string command, CancellationToken cancellationToken)
        {
            if (elements == null || !elements.Any()) return;

            await transaction.Connection.BulkInsertAsync(command, elements, transaction, cancellationToken: cancellationToken);
        }

        private async static Task StoreErrorDeclaration(CaptureDocumentRequest request, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var eventsWithErrorDeclaration = request.EventList.Where(x => x.ErrorDeclaration != null);

            var declarations = eventsWithErrorDeclaration.Select(e => { e.ErrorDeclaration.EventId = e.Id; return e; });
            var corrective = eventsWithErrorDeclaration.SelectMany(x => { x.ErrorDeclaration.CorrectiveEventIds.ForEach(r => r.EventId = x.Id); return x.ErrorDeclaration.CorrectiveEventIds; });

            await transaction.Connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreErrorDeclaration, declarations, transaction, cancellationToken: cancellationToken);
            await transaction.Connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreErrorDeclarationIds, corrective, transaction, cancellationToken: cancellationToken);
        }
    }
}
