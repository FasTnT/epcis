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
            = new StoreAction[]{ StoreRequest, StoreEvents, StoreEpcs, StoreCustomFields, StoreSourceDestinations, StoreBusinessTransactions, StoreErrorDeclaration };

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
            var namespaceStore = new NamespaceStore(unitOfWork);
            var customFieldsList = new List<CustomFieldDTO>();
            foreach (var x in request.EventList)
            {
                foreach (var f in x.CustomFields)
                {
                    f.EventId = x.Id;
                    var dto = new CustomFieldDTO
                    {
                        EventId = f.EventId,
                        Id = f.Id,
                        Type = f.Type,
                        Name = f.Name,
                        Namespace = f.Namespace,
                        TextValue = f.TextValue,
                        NumericValue = f.NumericValue,
                        DateValue = f.DateValue,
                        ParentId = f.ParentId,
                    };
                    var namespaceDTO = await namespaceStore.FindByName(dto.Namespace);
                    if (namespaceDTO == null)
                    {
                        namespaceDTO = await namespaceStore.Create(dto.Namespace);
                    }

                    dto.NamespaceId = namespaceDTO.Id;
                    customFieldsList.Add(dto);
                }
            }
            await unitOfWork.Execute(SqlRequests.StoreCustomField, customFieldsList);
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

        private async static Task StoreErrorDeclaration(EpcisEventDocument request, IUnitOfWork unitOfWork)
        {
            var eventsWithErrorDeclaration = request.EventList.Where(x => x.ErrorDeclaration != null);

            eventsWithErrorDeclaration.ForEach(x => x.ErrorDeclaration.EventId = x.Id);
            eventsWithErrorDeclaration.ForEach(x => x.ErrorDeclaration.CorrectiveEventIds.ForEach(t => t.EventId = x.Id));

            await unitOfWork.Execute(SqlRequests.StoreErrorDeclaration, eventsWithErrorDeclaration.Select(x => x.ErrorDeclaration));
            await unitOfWork.Execute(SqlRequests.StoreErrorDeclarationIds, eventsWithErrorDeclaration.SelectMany(x => x.ErrorDeclaration.CorrectiveEventIds));
        }
    }
    internal class CustomFieldDTO
    {
        public Guid EventId { get; set; }
        public int Id { get; set; }
        public FieldType Type { get; set; }
        public string Name { get; set; }
        public int NamespaceId { get; set; }
        public string Namespace { get; set; }
        public string TextValue { get; set; }
        public double? NumericValue { get; set; }
        public DateTime? DateValue { get; set; }
        public int? ParentId { get; set; }
    }
}
