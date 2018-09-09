using FasTnT.Domain;
using FasTnT.Model.Queries;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace FasTnT.Host.Binders
{
    public class EpcisInputBinderProvider : IModelBinderProvider
    {
        public IDictionary<Type, IModelBinder> Mappings = new Dictionary<Type, IModelBinder>
        {
            { typeof(Request),    new EpcisRequestModelBinder() },
            { typeof(EpcisQuery), new EpcisQueryModelBinder()   }
        };

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return Mappings[context.Metadata.ModelType];
        }
    }
}
