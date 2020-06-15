using Dapper;
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
    internal class EventDtoManager
    {
        public List<EventDto> Events { get; } = new List<EventDto>();
        public List<EpcDto> Epcs { get; } = new List<EpcDto>();
        public List<SourceDestDto> SourceDests { get; } = new List<SourceDestDto>();
        public List<TransactionDto> Transactions { get; } = new List<TransactionDto>();
        public List<CustomFieldDto> CustomFields { get; } = new List<CustomFieldDto>();
        public List<CorrectiveIdDto> CorrectiveIds { get; } = new List<CorrectiveIdDto>();

        internal void AddEvent(int requestId, short eventId, EpcisEvent currentEvent)
        {
            Events.Add(EventDto.Create(currentEvent, eventId, requestId));
            Epcs.AddRange(currentEvent.Epcs.Select(x => EpcDto.Create(x, eventId, requestId)));
            SourceDests.AddRange(currentEvent.SourceDestinationList.Select(x => SourceDestDto.Create(x, eventId, requestId)));
            Transactions.AddRange(currentEvent.BusinessTransactions.Select(x => TransactionDto.Create(x, eventId, requestId)));
            CorrectiveIds.AddRange(currentEvent.CorrectiveEventIds?.Select(x => CorrectiveIdDto.Create(x, eventId, requestId)));
            CustomFields.AddRange(currentEvent.CustomFields.ToFlattenedDtos(eventId, requestId));
        }

        internal async Task PersistAsync(IDbTransaction transaction, CancellationToken cancellationToken)
        {
            await transaction.BulkInsertAsync(Events, cancellationToken);
            await transaction.BulkInsertAsync(Epcs, cancellationToken);
            await transaction.BulkInsertAsync(SourceDests, cancellationToken);
            await transaction.BulkInsertAsync(Transactions, cancellationToken);
            await transaction.BulkInsertAsync(CustomFields, cancellationToken);
            await transaction.BulkInsertAsync(CorrectiveIds, cancellationToken);
        }

        internal static async Task<EventDtoManager> ReadAsync(SqlMapper.GridReader reader)
        {
            var holder = new EventDtoManager();

            holder.Events.AddRange(await reader.ReadAsync<EventDto>());
            holder.Epcs.AddRange(await reader.ReadAsync<EpcDto>());
            holder.CustomFields.AddRange(await reader.ReadAsync<CustomFieldDto>());
            holder.Transactions.AddRange(await reader.ReadAsync<TransactionDto>());
            holder.SourceDests.AddRange(await reader.ReadAsync<SourceDestDto>());
            holder.CorrectiveIds.AddRange(await reader.ReadAsync<CorrectiveIdDto>());

            return holder;
        }

        private static List<CustomField> CreateHierarchy(IEnumerable<CustomFieldDto> fieldsDtos, short? parentId = null)
        {
            var elements = fieldsDtos.Where(x => x.ParentId == parentId);
            var customFields = new List<CustomField>();

            foreach (var element in elements)
            {
                var field = element.ToCustomField();
                field.Children = CreateHierarchy(fieldsDtos, element.Id);
            }

            return customFields;
        }

        internal IEnumerable<EpcisEvent> FormatEvents()
        {
            return Events.Select(evt =>
            {
                var epcisEvent = evt.ToEpcisEvent();
                epcisEvent.Epcs = Epcs.Where(x => x.Matches(evt)).Select(x => x.ToEpc()).ToList();
                epcisEvent.CustomFields = CreateHierarchy(CustomFields.Where(x => x.Matches(evt)));
                epcisEvent.BusinessTransactions = Transactions.Where(x => x.Matches(evt)).Select(x => x.ToBusinessTransaction()).ToList();
                epcisEvent.SourceDestinationList = SourceDests.Where(x => x.Matches(evt)).Select(x => x.ToSourceDestination()).ToList();
                epcisEvent.CorrectiveEventIds = CorrectiveIds.Where(x => x.Matches(evt)).Select(x => x.ToCorrectiveId()).ToList();

                return epcisEvent;
            });
        }
    }
}
