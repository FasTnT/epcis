using FasTnT.Domain;
using FasTnT.Host.Infrastructure.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FasTnT.Host.Controllers.v1_2
{
    [Authorize]
    [Formatter(Format.Xml)]
    [Route("v1_2/Query")]
    public class EpcisXmlQueryController : EpcisQueryController
    {
        public EpcisXmlQueryController(QueryDispatcher dispatcher) : base(dispatcher)
        {
        }
    }
}
