using FasTnT.Commands.Responses;
using MediatR;

namespace FasTnT.Domain.Commands
{
    public interface ICaptureRequest : IRequest<IEpcisResponse> { }
}
