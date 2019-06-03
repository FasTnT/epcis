using FasTnT.Formatters;
using FasTnT.Model;
using FasTnT.Model.Queries;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Infrastructure.Binding
{
    public class EpcisModelBinderProvider : IModelBinderProvider
    {
        static readonly IDictionary<Type, IModelBinder> KnownBinders = new Dictionary<Type, IModelBinder>
        {
            { typeof(EpcisQuery), new EpcisModelBinder<EpcisQuery>(x => x.ReadQuery) },
            { typeof(Request), new EpcisModelBinder<Request>(x => x.ReadRequest) }
        };

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return KnownBinders.TryGetValue(context.Metadata.ModelType, out IModelBinder binder)
                ? binder
                : null;
        }

        private class EpcisModelBinder<T> : IModelBinder
        {
            private readonly Func<IFormatter, Func<Stream, CancellationToken, Task<T>>> _selector;

            public EpcisModelBinder(Func<IFormatter, Func<Stream, CancellationToken, Task<T>>> selector) => _selector = selector;

            public async Task BindModelAsync(ModelBindingContext bindingContext)
            {
                var httpCtx = bindingContext.HttpContext;
                var formatter = _selector(httpCtx.GetFormatter());
                var model = await formatter(httpCtx.Request.Body, httpCtx.RequestAborted);

                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }
    }
}
