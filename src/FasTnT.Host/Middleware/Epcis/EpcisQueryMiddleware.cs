using Microsoft.Extensions.Logging;
using FasTnT.Domain.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using System;
using FasTnT.Model.Responses;

namespace FasTnT.Host
{
    internal class EpcisQueryMiddleware : EpcisMiddleware<EpcisQuery>
    {
        public EpcisQueryMiddleware(ILogger<EpcisQueryMiddleware> logger, RequestDelegate next, string path)
            : base(logger, next, path) { }

        public override async Task Process(EpcisQuery parameter)
        {
            if (parameter is GetQueryNames getQueryNames)
                await Execute(async s => await s.Process(getQueryNames));
            else if (parameter is GetSubscriptionIds getSubscriptionIds)
                await Execute(async s => await s.Process(getSubscriptionIds));
            else if (parameter is Poll poll)
                await Execute(async s => await s.Process(poll));
            else if (parameter is GetStandardVersion getStandardVersion)
                await Execute(async s => await s.Process(getStandardVersion));
            else if (parameter is GetVendorVersion getVendorVersion)
                await Execute(async s => await s.Process(getVendorVersion));
            else if (parameter is Subscription subscription)
                await Execute(async s => await s.Process(subscription));
            else if (parameter is UnsubscribeRequest unsubscribeRequest)
                await Execute(async s => await s.Process(unsubscribeRequest));
        }

        private async Task Execute(Func<QueryService, Task<IEpcisResponse>> action) => await Execute<QueryService>(async s => await action(s));
        private async Task Execute(Func<QueryService, Task> action) => await Execute<QueryService>(async s => await action(s));
    }
}
