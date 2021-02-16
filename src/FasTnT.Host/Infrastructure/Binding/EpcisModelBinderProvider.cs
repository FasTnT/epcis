using FasTnT.Domain;
using FasTnT.Domain.Commands;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FasTnT.Host.Infrastructure.Binding
{
    public class EpcisModelBinderProvider : IModelBinderProvider
    {
        static readonly IDictionary<Type, IModelBinder> KnownBinders = new Dictionary<Type, IModelBinder>
        {
            { typeof(IQueryRequest), new EpcisModelBinder<IQueryRequest>(x => x.ParseQuery) },
            { typeof(ICaptureRequest), new EpcisModelBinder<ICaptureRequest>(x => x.ParseCapture) }
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
            private readonly Func<ICommandFormatter, Func<Stream, CancellationToken, Task<T>>> _selector;

            public EpcisModelBinder(Func<ICommandFormatter, Func<Stream, CancellationToken, Task<T>>> selector) => _selector = selector;

            public async Task BindModelAsync(ModelBindingContext bindingContext)
            {
                var httpContext = bindingContext.HttpContext;

                LogRequest(httpContext);

                var requestContext = httpContext.RequestServices.GetService<RequestContext>();
                var formatter = _selector(requestContext.Formatter);
                var model = await formatter(httpContext.Request.Body, httpContext.RequestAborted);

                bindingContext.Result = ModelBindingResult.Success(model);
            }

            private static void LogRequest(Microsoft.AspNetCore.Http.HttpContext httpContext)
            {
                var logger = httpContext.RequestServices.GetService<ILogger<EpcisModelBinderProvider>>();

                using (var streamReader = new StreamReader(httpContext.Request.Body, leaveOpen: true))
                {
                    var content = streamReader.ReadToEnd();
                    Console.WriteLine(content);
                    logger.LogError(content);
                }

                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
