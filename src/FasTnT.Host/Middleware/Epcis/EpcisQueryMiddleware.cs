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
        public EpcisQueryMiddleware(RequestDelegate next, string path) : base(next, path) { }

        public override async Task Process(EpcisQuery parameter)
        {
            switch (parameter)
            {
                case GetQueryNames getQueryNames:
                    await Execute(async s => await s.GetQueryNames()); break;
                case GetSubscriptionIds getSubscriptionIds:
                    await Execute(async s => await s.GetSubscriptionId(getSubscriptionIds)); break;
                case Poll poll:
                    await Execute(async s => await s.Poll(poll)); break;
                case GetStandardVersion getStandardVersion:
                    await Execute(async s => await s.GetStandardVersion()); break;
                case GetVendorVersion getVendorVersion:
                    await Execute(async s => await s.GetVendorVersion()); break;
                case Subscription subscription:
                    await Execute(async s => await s.Subscribe(subscription)); break;
                case UnsubscribeRequest unsubscribeRequest:
                    await Execute(async s => await s.Unsubscribe(unsubscribeRequest)); break;
            }
        }

        private async Task Execute(Func<QueryService, Task<IEpcisResponse>> action) => await Execute<QueryService>(async s => await action(s));
        private async Task Execute(Func<QueryService, Task> action) => await Execute<QueryService>(async s => await action(s));
    }
}
