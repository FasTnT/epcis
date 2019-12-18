using FasTnT.Commands.Responses;
using FasTnT.Model;
using MediatR;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisExceptionRequest : IRequest<IEpcisResponse>
    {
        public EpcisRequestHeader Header { get; set; }
        public EpcisQueryCallbackException Exception { get; set; }
    }
}
