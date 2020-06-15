using FasTnT.Data.PostgreSql.DTOs;
using FasTnT.Domain;
using FasTnT.Domain.Data;
using FasTnT.Model;
using FasTnT.Model.Events;
using FasTnT.Model.Headers;
using FasTnT.Model.Users;
using FasTnT.PostgreSql.DapperConfiguration;
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

                if (request.StandardBusinessHeader != null)
                {
                    await StoreStandardBusinessHeader(request.StandardBusinessHeader, requestId, tx, cancellationToken);
                }
                if (request.SubscriptionCallback != null)
                {
                    await StoreCallbackInformation(request.SubscriptionCallback, requestId, tx, cancellationToken);
                }
                if (request.EventList.Any()) 
                {
                    await StoreEpcisEvents(request.EventList.ToArray(), tx, requestId, cancellationToken);
                }
                if (request.MasterdataList.Any())
                {
                    await EpcisMasterdataStore.StoreEpcisMasterdata(request.MasterdataList.ToArray(), tx, cancellationToken);
                }

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
            var headerDto = StandardHeaderDto.Create(header, requestId);
            var contacts = header.ContactInformations.Select((x, i) => ContactInformationDto.Create(x, requestId, i));

            await transaction.InsertAsync(headerDto, cancellationToken);
            await transaction.BulkInsertAsync(contacts, cancellationToken);
        }

        private static async Task StoreCallbackInformation(SubscriptionCallback callback, int requestId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var parameters = SubscriptionCallbackDto.Create(callback, requestId);

            await transaction.InsertAsync(parameters, cancellationToken);
        }

        private static async Task StoreEpcisEvents(EpcisEvent[] events, IDbTransaction transaction, int requestId, CancellationToken cancellationToken)
        {
            var eventDtoManager = new EventDtoManager();

            for (short eventId = 0; eventId < events.Length; eventId++)
            {
                eventDtoManager.AddEvent(requestId, eventId, events[eventId]);
            }

            await eventDtoManager.PersistAsync(transaction, cancellationToken);
        }
    }
}
