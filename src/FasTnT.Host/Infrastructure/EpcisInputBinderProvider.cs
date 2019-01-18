using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Subscriptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace FasTnT.Host.Binders
{
    public class EpcisInputBinderProvider : IModelBinderProvider
    {
        public IDictionary<Type, IModelBinder> Mappings = new Dictionary<Type, IModelBinder>
        {
            { typeof(EpcisEventDocument), new EpcisRequestModelBinder() },
            { typeof(EpcisMasterdataDocument), new EpcisRequestModelBinder() },
            { typeof(Poll), new EpcisQueryModelBinder() },
            { typeof(GetSubscriptionIds), new EpcisQueryModelBinder() },
            { typeof(GetQueryNames), new EpcisQueryModelBinder() },
            { typeof(GetStandardVersion), new EpcisQueryModelBinder() },
            { typeof(GetVendorVersion), new EpcisQueryModelBinder() },
            { typeof(Subscribe), new EpcisSubscriptionModelBinder() },
            { typeof(UnsubscribeRequest), new EpcisSubscriptionModelBinder() }
        };

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return Mappings.TryGetValue(context.Metadata.ModelType, out IModelBinder binder)
                ? binder
                : null;
        }
    }
}
