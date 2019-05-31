using FasTnT.Model;
using FasTnT.Model.Queries;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace FasTnT.Host.Infrastructure.Binding
{
    public class AbstractModelBinderProvider : IModelBinderProvider
    {
        static IDictionary<Type, IModelBinder> KnownBinders = new Dictionary<Type, IModelBinder>
        {
            { typeof(EpcisQuery), new EpcisQueryModelBinder() },
            { typeof(Request), new EpcisRequestModelBinder() }
        };

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return KnownBinders.TryGetValue(context.Metadata.ModelType, out IModelBinder binder)
                ? binder
                : null;
        }
    }
}
