using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using System;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Handlers
{
    public class SubscribeHandler : IQueryHandler<Subscribe>
    {
        public Task<IEpcisResponse> Handle(Subscribe subscribe)
        {
            throw new NotImplementedException();
        }
    }
}
