using FasTnT.Commands.Responses;
using MediatR;

namespace FasTnT.Domain.Commands
{
    public interface IQueryRequest : IRequest<IEpcisResponse> { }
}
