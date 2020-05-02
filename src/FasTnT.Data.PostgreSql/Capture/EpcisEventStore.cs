using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Data.PostgreSql.Utils;
using FasTnT.Model.Events;
using MoreLinq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoreAction = System.Func<FasTnT.Model.Events.EpcisEvent[], System.Data.IDbTransaction, System.Threading.CancellationToken, System.Threading.Tasks.Task>;

namespace FasTnT.PostgreSql.Capture
{
    public static class EpcisEventStore
    {
        private static readonly StoreAction[] Actions = new StoreAction[] { StoreEvents, StoreEpcs, StoreCustomFields, StoreSourceDestinations, StoreBusinessTransactions, StoreErrorDeclaration };

        public static async Task StoreEpcisEvents(IEnumerable<EpcisEvent> events, IDbTransaction transaction, int headerId, CancellationToken cancellationToken)
        {
            if (events == null || !events.Any()) return;

            var eventList = events.Select(e => { e.RequestId = headerId; return e; }).ToArray();

            foreach (var action in Actions)
            {
                await action(eventList, transaction, cancellationToken);
            }
        }

        private async static Task StoreEvents(EpcisEvent[] events, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var eventIds = await transaction.Connection.BulkInsertReturningAsync(CaptureEpcisEventCommand.StoreEvent, events, transaction, cancellationToken: cancellationToken);
            var idArray = eventIds.ToArray();

            for (var i = 0; i < events.Length; i++)
            {
                events[i].Id = idArray[i];
            }
        }

        private async static Task StoreCustomFields(EpcisEvent[] events, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var fields = new List<CustomField>();
            events.ForEach(evt => PgSqlCustomFieldsParser.ParseFields(evt.CustomFields, evt.Id.Value, fields));

            await transaction.Connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreCustomField, fields, transaction, cancellationToken: cancellationToken);
        }

        private async static Task StoreEpcs(EpcisEvent[] events, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            await Store(transaction, events.SelectMany(e => { e.Epcs.ForEach(x => x.EventId = e.Id); return e.Epcs; }), CaptureEpcisEventCommand.StoreEpcs, cancellationToken);
        }
        private async static Task StoreSourceDestinations(EpcisEvent[] events, IDbTransaction transaction, CancellationToken cancellationToken)
        { 
            await Store(transaction, events.SelectMany(e => { e.SourceDestinationList.ForEach(sd => sd.EventId = e.Id); return e.SourceDestinationList; }), CaptureEpcisEventCommand.StoreSourceDestination, cancellationToken); 
        }
        private async static Task StoreBusinessTransactions(EpcisEvent[] events, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            await Store(transaction, events.SelectMany(e => { e.BusinessTransactions.ForEach(x => x.EventId = e.Id); return e.BusinessTransactions; }), CaptureEpcisEventCommand.StoreBusinessTransaction, cancellationToken);
        }

        private async static Task Store<T>(IDbTransaction transaction, IEnumerable<T> elements, string command, CancellationToken cancellationToken)
        {
            if (elements == null || !elements.Any()) return;

            await transaction.Connection.BulkInsertAsync(command, elements, transaction, cancellationToken: cancellationToken);
        }

        private async static Task StoreErrorDeclaration(EpcisEvent[] events, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var eventsWithErrorDeclaration = events.Where(x => x.ErrorDeclaration != null);

            var declarations = eventsWithErrorDeclaration.Select(e => { e.ErrorDeclaration.EventId = e.Id; return e; });
            var corrective = eventsWithErrorDeclaration.SelectMany(x => { x.ErrorDeclaration.CorrectiveEventIds.ForEach(r => r.EventId = x.Id); return x.ErrorDeclaration.CorrectiveEventIds; });

            await transaction.Connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreErrorDeclaration, declarations, transaction, cancellationToken: cancellationToken);
            await transaction.Connection.BulkInsertAsync(CaptureEpcisEventCommand.StoreErrorDeclarationIds, corrective, transaction, cancellationToken: cancellationToken);
        }
    }
}
