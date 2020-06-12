using FasTnT.Data.PostgreSql.DapperConfiguration;
using FasTnT.Data.PostgreSql.DTOs;
using FasTnT.Model.Events;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.DapperConfiguration
{
    internal class EventDtoHolder
    {
        public List<EventDto> Events { get; }
        public List<EpcDto> Epcs { get; }
        public List<SourceDestDto> SourceDests { get; }
        public List<TransactionDto> Transactions { get; }
        public List<CustomFieldDto> CustomFields { get; }
        public List<CorrectiveIdDto> CorrectiveIds { get; }

        public EventDtoHolder(int length)
        {
            Events = new List<EventDto>(length);
            Epcs = new List<EpcDto>();
            SourceDests = new List<SourceDestDto>();
            Transactions = new List<TransactionDto>();
            CustomFields = new List<CustomFieldDto>();
            CorrectiveIds = new List<CorrectiveIdDto>();
        }

        internal void AddEvent(int requestId, short eventId, EpcisEvent currentEvent)
        {
            Events.Add(EventDto.Create(currentEvent, eventId, requestId));
            Epcs.AddRange(currentEvent.Epcs.Select(x => EpcDto.Create(x, eventId, requestId)));
            SourceDests.AddRange(currentEvent.SourceDestinationList.Select(x => SourceDestDto.Create(x, eventId, requestId)));
            Transactions.AddRange(currentEvent.BusinessTransactions.Select(x => TransactionDto.Create(x, eventId, requestId)));
            CorrectiveIds.AddRange(currentEvent.CorrectiveEventIds?.Select(x => CorrectiveIdDto.Create(x, eventId, requestId)));
            CustomFields.AddRange(currentEvent.CustomFields.ToFlattenedDtos(eventId, requestId));
        }

        internal async Task StoreEventsAsync(IDbTransaction transaction, CancellationToken cancellationToken)
        {
            await transaction.BulkInsertAsync(Events, cancellationToken);
            await transaction.BulkInsertAsync(Epcs, cancellationToken);
            await transaction.BulkInsertAsync(SourceDests, cancellationToken);
            await transaction.BulkInsertAsync(Transactions, cancellationToken);
            await transaction.BulkInsertAsync(CustomFields, cancellationToken);
            await transaction.BulkInsertAsync(CorrectiveIds, cancellationToken);
        }

        internal async Task ReadEventsAsync(IDbTransaction transaction, CancellationToken cancellationToken)
        {
            await transaction.BulkInsertAsync(Events, cancellationToken);
            await transaction.BulkInsertAsync(Epcs, cancellationToken);
            await transaction.BulkInsertAsync(SourceDests, cancellationToken);
            await transaction.BulkInsertAsync(Transactions, cancellationToken);
            await transaction.BulkInsertAsync(CustomFields, cancellationToken);
            await transaction.BulkInsertAsync(CorrectiveIds, cancellationToken);
        }
    }
}
