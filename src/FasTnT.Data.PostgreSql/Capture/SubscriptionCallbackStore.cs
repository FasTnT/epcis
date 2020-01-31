using Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Domain.Data.Model;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.Capture
{
    public class SubscriptionCallbackStore
    {
        public static async Task Store(CaptureCallbackRequest callback, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var parameters = new
            {
                callback.CallbackType,
                callback.SubscriptionId,
                RequestId = headerId
            };

            await transaction.Connection.ExecuteAsync(new CommandDefinition(CaptureEpcisCallbackCommands.StoreCallback, parameters, transaction, cancellationToken: cancellationToken));
        }
    }
}
