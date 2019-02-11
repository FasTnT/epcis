using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Model;
using MoreLinq;
using StoreAction = System.Func<FasTnT.Model.EpcisEvent[], FasTnT.Persistence.Dapper.DapperUnitOfWork, System.Threading.Tasks.Task>;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlEventStore : IEventStore
    {
        private readonly DapperUnitOfWork _unitOfWork;
        private readonly IEnumerable<StoreAction> _actions = new StoreAction[]{ StoreEvents, StoreEpcs, StoreCustomFields, StoreSourceDestinations, StoreBusinessTransactions, StoreErrorDeclaration };

        public PgSqlEventStore(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Store(Guid eventsId, EpcisEvent[] events)
        {
            events.ForEach(x => x.RequestId = eventsId);

            foreach (var action in _actions)
            {
                await action(events, _unitOfWork);
            }
        }

        private async static Task StoreEvents(EpcisEvent[] events, DapperUnitOfWork unitOfWork)
        {
            await unitOfWork.Execute(SqlRequests.StoreEvent, events);
        }

        private async static Task StoreEpcs(EpcisEvent[] events, DapperUnitOfWork unitOfWork)
        {
            events.ForEach(x => x.Epcs.ForEach(e => e.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreEpcs, events.SelectMany(x => x.Epcs));
        }

        private async static Task StoreCustomFields(EpcisEvent[] events, DapperUnitOfWork unitOfWork)
        {
            events.ForEach(x => x.CustomFields.ForEach(f => f.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreCustomField, events.SelectMany(x => x.CustomFields));
        }

        private async static Task StoreSourceDestinations(EpcisEvent[] events, DapperUnitOfWork unitOfWork)
        {
            events.ForEach(x => x.SourceDestinationList.ForEach(s => s.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreSourceDestination, events.SelectMany(x => x.SourceDestinationList));
        }

        private async static Task StoreBusinessTransactions(EpcisEvent[] events, DapperUnitOfWork unitOfWork)
        {
            events.ForEach(x => x.BusinessTransactions.ForEach(t => t.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreBusinessTransaction, events.SelectMany(x => x.BusinessTransactions));
        }

        private async static Task StoreErrorDeclaration(EpcisEvent[] events, DapperUnitOfWork unitOfWork)
        {
            var eventsWithErrorDeclaration = events.Where(x => x.ErrorDeclaration != null);

            eventsWithErrorDeclaration.ForEach(x => x.ErrorDeclaration.EventId = x.Id);
            eventsWithErrorDeclaration.ForEach(x => x.ErrorDeclaration.CorrectiveEventIds.ForEach(t => t.EventId = x.Id));

            await unitOfWork.Execute(SqlRequests.StoreErrorDeclaration, eventsWithErrorDeclaration.Select(x => x.ErrorDeclaration));
            await unitOfWork.Execute(SqlRequests.StoreErrorDeclarationIds, eventsWithErrorDeclaration.SelectMany(x => x.ErrorDeclaration.CorrectiveEventIds));
        }
    }
}
