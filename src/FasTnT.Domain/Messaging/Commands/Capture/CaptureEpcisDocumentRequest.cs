using FasTnT.Domain.Commands;
using FasTnT.Model;

namespace FasTnT.Commands.Requests
{
    public class CaptureEpcisDocumentRequest : ICaptureRequest
    {
        public EpcisRequest Request { get; init; }
    }
}
