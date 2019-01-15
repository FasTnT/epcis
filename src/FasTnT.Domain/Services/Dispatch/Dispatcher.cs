using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Domain.Services.Handlers;
using FasTnT.Model;
using FasTnT.Model.Subscriptions;

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

            return await DispatchInternal(handlerType, document);
        }

        public async Task<IEpcisResponse> Dispatch(EpcisQuery query)
        {
            var handlerType = _handlers.SingleOrDefault(x => x.GetInterfaces().SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<>))?.GetGenericArguments()[0] == query.GetType());

            return await DispatchInternal(handlerType, query);
        }

        public async Task<IEpcisResponse> Dispatch(SubscriptionRequest request)
        {
            var handlerType = _handlers.SingleOrDefault(x => x.GetInterfaces().SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriptionHandler<>))?.GetGenericArguments()[0] == request.GetType());

            return await DispatchInternal(handlerType, request);
        }

        private async Task<IEpcisResponse> DispatchInternal(Type handlerType, params object[] parameters)
        {
            if(handlerType == null)
            {
                throw new NotImplementedException();
            }

            var handler = _serviceProvider.GetService(handlerType);
            return await (handler.GetType().GetTypeInfo().GetDeclaredMethod("Handle").Invoke(handler, parameters) as Task<IEpcisResponse>);
        }
    }
}
