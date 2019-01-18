using FasTnT.Formatters.Xml;
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
        static readonly EpcisModelBinder<Request> RequestBinder = new EpcisModelBinder<Request>(new XmlRequestFormatter(), null); 
        static readonly EpcisModelBinder<EpcisQuery> QueryBinder = new EpcisModelBinder<EpcisQuery>(new XmlQueryFormatter(), null); 
        static readonly EpcisModelBinder<SubscriptionRequest> SubscriptionBinder = new EpcisModelBinder<SubscriptionRequest>(new XmlSubscriptionFormatter(), null); 

        public IDictionary<Type, IModelBinder> Mappings = new Dictionary<Type, IModelBinder>
        {
            { typeof(Request), RequestBinder },
            { typeof(EpcisQuery), QueryBinder },
            { typeof(SubscriptionRequest), SubscriptionBinder }
        };

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return Mappings.TryGetValue(context.Metadata.ModelType?.BaseType, out IModelBinder binder) ? binder : null;
        }
    }
}
