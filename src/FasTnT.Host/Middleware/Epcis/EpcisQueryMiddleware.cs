using Microsoft.Extensions.Logging;
using FasTnT.Domain.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;

namespace FasTnT.Host
{

    internal class EpcisQueryMiddleware : EpcisMiddleware<EpcisQuery>
    {
        public EpcisQueryMiddleware(ILogger<EpcisCaptureMiddleware> logger, RequestDelegate next, string path)
            : base(logger, next, path) { }

        public override async Task Process(EpcisQuery parameter)
        {
            if (parameter is GetQueryNames getQueryNames)
                await Write<QueryService>(async s => await s.Process(getQueryNames));
            else if (parameter is GetSubscriptionIds getSubscriptionIds)
                await Write<QueryService>(async s => await s.Process(getSubscriptionIds));
            else if (parameter is Poll poll)
                await Write<QueryService>(async s => await s.Process(poll));
            else if (parameter is GetStandardVersion getStandardVersion)
                await Write<QueryService>(async s => await s.Process(getStandardVersion));
            else if (parameter is GetVendorVersion getVendorVersion)
                await Write<QueryService>(async s => await s.Process(getVendorVersion));
            else if (parameter is Subscription subscription)
                await Created<QueryService>(async s => await s.Process(subscription));
            else if (parameter is UnsubscribeRequest unsubscribeRequest)
                await Created<QueryService>(async s => await s.Process(unsubscribeRequest));
        }
    }
}
