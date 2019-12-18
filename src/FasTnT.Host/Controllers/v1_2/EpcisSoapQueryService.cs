using FasTnT.Host.Infrastructure.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FasTnT.Host.Controllers.v1_2
{
    [Authorize]
    [Formatter(Format.Soap)]
    [Route("v1_2/Query.svc")]
    public class EpcisSoapQueryService : EpcisQueryController
    {
        public EpcisSoapQueryService(IMediator dispatcher) : base(dispatcher)
        {
        }
    }
}
