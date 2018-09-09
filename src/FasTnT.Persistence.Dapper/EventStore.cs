using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FasTnT.Domain;
using FasTnT.Domain.Persistence;
using MoreLinq;
using StoreAction = System.Func<FasTnT.Domain.EpcisEventDocument, FasTnT.Persistence.Dapper.IUnitOfWork, System.Threading.Tasks.Task>;

namespace FasTnT.Persistence.Dapper
{
    public class EventStore : IEventStore
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<StoreAction> _actions 
            = new StoreAction[]{ StoreRequest, StoreEvents, StoreEpcs, StoreCustomFields, StoreSourceDestinations, StoreBusinessTransactions };

        public EventStore(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task Store(EpcisEventDocument request)
        {
            using (new CommitOnDisposeScope(_unitOfWork))
            {
                foreach (var action in _actions)
                {
                    await action(request, _unitOfWork);
                }
            }
        }

        private async static Task StoreRequest(EpcisEventDocument request, IUnitOfWork unitOfWork)
        {
            await unitOfWork.Execute(SqlRequests.StoreRequest, new { request.Id, DocumentTime = request.CreationDate, RecordTime = DateTime.UtcNow });
        }

        private async static Task StoreEvents(EpcisEventDocument request, IUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.RequestId = request.Id);
            await unitOfWork.Execute(SqlRequests.StoreEvent, request.EventList);
        }

        private async static Task StoreEpcs(EpcisEventDocument request, IUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.Epcs.ForEach(e => e.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreEpcs, request.EventList.SelectMany(x => x.Epcs));
        }

        private async static Task StoreCustomFields(EpcisEventDocument request, IUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.CustomFields.ForEach(f => f.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreCustomField, request.EventList.SelectMany(x => x.CustomFields));
        }

        private async static Task StoreSourceDestinations(EpcisEventDocument request, IUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.SourceDestinationList.ForEach(s => s.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreSourceDestination, request.EventList.SelectMany(x => x.SourceDestinationList));
        }

        private async static Task StoreBusinessTransactions(EpcisEventDocument request, IUnitOfWork unitOfWork)
        {
            request.EventList.ForEach(x => x.BusinessTransactions.ForEach(t => t.EventId = x.Id));
            await unitOfWork.Execute(SqlRequests.StoreBusinessTransaction, request.EventList.SelectMany(x => x.BusinessTransactions));
        }
    }
}
