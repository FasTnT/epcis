using Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Model;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.Capture
{
    public class SubscriptionCallbackStore
    {
        public static async Task Store(EpcisRequest request, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            if (request.SubscriptionCallback == null) return;
         
            var parameters = new
            {
                request.SubscriptionCallback.CallbackType,
                request.SubscriptionCallback.SubscriptionId,
                RequestId = headerId
            };

            await transaction.Connection.ExecuteAsync(new CommandDefinition(CaptureEpcisCallbackCommands.StoreCallback, parameters, transaction, cancellationToken: cancellationToken));
        }
    }
}
