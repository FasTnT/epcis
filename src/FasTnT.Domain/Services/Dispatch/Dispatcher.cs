using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Domain.Services.Handlers;

namespace FasTnT.Domain.Services.Dispatch
{
    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Type[] _handlers;

        public Dispatcher(IServiceProvider serviceProvider, Type[] handlers)
        {
            _serviceProvider = serviceProvider;
            _handlers = handlers;
        }

        public async Task<IEpcisResponse> Dispatch(Request document)
        {
            var handlerType = _handlers.SingleOrDefault(x => x.GetInterfaces().SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<>))?.GetGenericArguments()[0] == document.GetType());
            var handler = _serviceProvider.GetService(handlerType);

            return await DispatchInternal(handler, document);
        }

        public async Task<IEpcisResponse> Dispatch(EpcisQuery query)
        {
            var handlerType = _handlers.SingleOrDefault(x => x.GetInterfaces().SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<>))?.GetGenericArguments()[0] == query.GetType());
            var handler = _serviceProvider.GetService(handlerType);

            return await DispatchInternal(handler, query);
        }

        private static async Task<IEpcisResponse> DispatchInternal(object handler, params object[] parameters) =>
            await (handler.GetType().GetTypeInfo().GetDeclaredMethod("Handle").Invoke(handler, parameters) as Task<IEpcisResponse>);
    }
}
