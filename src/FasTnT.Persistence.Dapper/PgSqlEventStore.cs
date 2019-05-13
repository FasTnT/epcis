using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model;
using MoreLinq;
using StoreAction = System.Func<FasTnT.Persistence.Dapper.EpcisEventEntity[], FasTnT.Persistence.Dapper.DapperUnitOfWork, System.Threading.CancellationToken, System.Threading.Tasks.Task>;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlEventStore : IEventStore
    {
        private readonly DapperUnitOfWork _unitOfWork;
        private readonly IEnumerable<StoreAction> _actions = new StoreAction[]{ StoreEvents, StoreEpcs, StoreCustomFields, StoreSourceDestinations, StoreBusinessTransactions, StoreErrorDeclaration };

        public PgSqlEventStore(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Store(Guid requestId, IEnumerable<EpcisEvent> events, CancellationToken cancellationToken)
        {
            var entities = events.Select(e => e.Map<EpcisEvent, EpcisEventEntity>(r => r.RequestId = requestId)).ToArray();

            foreach (var action in _actions)
            {
                await action(entities, _unitOfWork, cancellationToken);
            }
        }

        private async static Task StoreEvents(EpcisEventEntity[] events, DapperUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            await unitOfWork.Execute(SqlRequests.StoreEvent, events, cancellationToken);
        }

        private async static Task StoreEpcs(EpcisEventEntity[] events, DapperUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var epcs = events.SelectMany(e => e.Epcs.Select(x => x.Map<Epc, EpcEntity>(r => r.EventId = e.Id)));
            await unitOfWork.Execute(SqlRequests.StoreEpcs, epcs, cancellationToken);
        }

        private async static Task StoreCustomFields(EpcisEventEntity[] events, DapperUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var fields = new List<CustomFieldEntity>();
            events.ForEach(evt => ParseFields(evt.CustomFields, evt.Id, fields));

            await unitOfWork.Execute(SqlRequests.StoreCustomField, fields, cancellationToken);
        }

        private static void ParseFields(IList<CustomField> customFields, Guid eventId, List<CustomFieldEntity> mappedList, int? parentId = null)
        {
            if (customFields == null || !customFields.Any()) return;

            foreach(var field in customFields)
            {
                var entity = field.Map<CustomField, CustomFieldEntity>(f => { f.EventId = eventId; f.Id = mappedList.Count; f.ParentId = parentId; });
                mappedList.Add(entity);

                ParseFields(field.Children, eventId, mappedList, entity.Id);
            }
        }

        private async static Task StoreSourceDestinations(EpcisEventEntity[] events, DapperUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var sourceDest = events.SelectMany(e => e.SourceDestinationList.Select(x => x.Map<SourceDestination, SourceDestinationEntity>(r => r.EventId = e.Id)));
            await unitOfWork.Execute(SqlRequests.StoreSourceDestination, sourceDest, cancellationToken);
        }

        private async static Task StoreBusinessTransactions(EpcisEventEntity[] events, DapperUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var tx = events.SelectMany(e => e.BusinessTransactions.Select(x => x.Map<BusinessTransaction, BusinessTransactionEntity>(r => r.EventId = e.Id)));
            await unitOfWork.Execute(SqlRequests.StoreBusinessTransaction, tx, cancellationToken);
        }

        private async static Task StoreErrorDeclaration(EpcisEventEntity[] events, DapperUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var eventsWithErrorDeclaration = events.Where(x => x.ErrorDeclaration != null);

            var declarations = eventsWithErrorDeclaration.Select(e => e.ErrorDeclaration.Map<ErrorDeclaration, ErrorDeclarationEntity>(r => r.EventId = e.Id));
            var corrective = eventsWithErrorDeclaration.SelectMany(x => x.ErrorDeclaration.CorrectiveEventIds.Select(t => t.Map<CorrectiveEventId, CorrectiveEventIdEntity>(r => r.EventId = x.Id)));

            await unitOfWork.Execute(SqlRequests.StoreErrorDeclaration, declarations, cancellationToken);
            await unitOfWork.Execute(SqlRequests.StoreErrorDeclarationIds, corrective, cancellationToken);
        }
    }
}
