using FasTnT.Data.PostgreSql.DapperConfiguration;
using FasTnT.Data.PostgreSql.DTOs;
using FasTnT.Domain;
using FasTnT.Domain.Data;
using FasTnT.Model;
using FasTnT.Model.Events;
using FasTnT.Model.Headers;
using FasTnT.Model.MasterDatas;
using FasTnT.Model.Users;
using FasTnT.PostgreSql.DapperConfiguration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.Capture
{
    public class EpcisRequestStore : IEpcisRequestStore
    {
        private readonly IDbConnection _connection;

        public EpcisRequestStore(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task Capture(EpcisRequest request, RequestContext context, CancellationToken cancellationToken)
        {
            using (var tx = _connection.BeginTransaction())
            {
                var requestId = await StoreHeader(request, context.User, tx, cancellationToken);

                await StoreStandardBusinessHeader(request.StandardBusinessHeader, requestId, tx, cancellationToken);
                await StoreCallbackInformation(request.SubscriptionCallback, requestId, tx, cancellationToken);
                await StoreEpcisEvents(request.EventList, tx, requestId, cancellationToken);
                await StoreMasterData(request.MasterdataList, tx, cancellationToken);

                tx.Commit();
            };
        }

        private async Task<int> StoreHeader(EpcisRequest request, User user, IDbTransaction tx, CancellationToken cancellationToken)
        {
            var requestDto = RequestDto.Create(request, user.Id);

            return await tx.InsertAsync(requestDto, cancellationToken);
        }

        private async Task StoreStandardBusinessHeader(StandardBusinessHeader header, int requestId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            if (header == default) return;
            
            var headerDto = StandardHeaderDto.Create(header, requestId);
            var contacts = header.ContactInformations.Select((x, i) => ContactInformationDto.Create(x, requestId, i));

            await transaction.InsertAsync(headerDto, cancellationToken);
            await transaction.BulkInsertAsync(contacts, cancellationToken);
        }

        private static async Task StoreCallbackInformation(SubscriptionCallback callback, int requestId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            if (callback == default) return;

            var parameters = SubscriptionCallbackDto.Create(callback, requestId);

            await transaction.InsertAsync(parameters, cancellationToken);
        }

        private static async Task StoreEpcisEvents(List<EpcisEvent> events, IDbTransaction transaction, int requestId, CancellationToken cancellationToken)
        {
            if (events.Count == 0) return;

            var eventDtoManager = new EventDtoManager();

            for (short eventId = 0; eventId < events.Count; eventId++)
            {
                eventDtoManager.AddEvent(requestId, eventId, events[eventId]);
            }

            await eventDtoManager.PersistAsync(transaction, cancellationToken);
        }

        private async Task StoreMasterData(List<EpcisMasterData> epcisMasterDatas, IDbTransaction tx, CancellationToken cancellationToken)
        {
            if (epcisMasterDatas.Count == 0) return;

            var masterDataDtoManager = new MasterdataDtoManager();

            for (short masterdataId = 0; masterdataId < epcisMasterDatas.Count; masterdataId++)
            {
                masterDataDtoManager.AddMasterdata(epcisMasterDatas[masterdataId]);
            }

            await masterDataDtoManager.PersistAsync(tx, cancellationToken);
        }
    }
}
