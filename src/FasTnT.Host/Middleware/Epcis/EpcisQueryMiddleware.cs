using FasTnT.Domain.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using System;
using FasTnT.Model.Responses;
using System.Threading;

namespace FasTnT.Host
{
    internal class EpcisQueryMiddleware : EpcisMiddleware<EpcisQuery>
    {
        public EpcisQueryMiddleware(RequestDelegate next, string path)
            : base(next, path) { }

        public override async Task Process(EpcisQuery parameter, CancellationToken cancellationToken)
        {
            if (parameter is GetQueryNames getQueryNames)
                await Execute(async s => await s.GetQueryNames(cancellationToken), cancellationToken);
            else if (parameter is GetSubscriptionIds getSubscriptionIds)
                await Execute(async s => await s.GetSubscriptionId(getSubscriptionIds, cancellationToken), cancellationToken);
            else if (parameter is Poll poll)
                await Execute(async s => await s.Poll(poll, cancellationToken), cancellationToken);
            else if (parameter is GetStandardVersion getStandardVersion)
                await Execute(async s => await s.GetStandardVersion(cancellationToken), cancellationToken);
            else if (parameter is GetVendorVersion getVendorVersion)
                await Execute(async s => await s.GetVendorVersion(cancellationToken), cancellationToken);
            else if (parameter is Subscription subscription)
                await Execute(async s => await s.Subscribe(subscription, cancellationToken));
            else if (parameter is UnsubscribeRequest unsubscribeRequest)
                await Execute(async s => await s.Unsubscribe(unsubscribeRequest, cancellationToken));
        }

        private async Task Execute(Func<QueryService, Task<IEpcisResponse>> action, CancellationToken cancellationToken) => await Execute<QueryService>(async s => await action(s), cancellationToken);
        private async Task Execute(Func<QueryService, Task> action) => await Execute<QueryService>(async s => await action(s));
    }
}
