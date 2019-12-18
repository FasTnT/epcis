using FasTnT.Commands.Responses;
using MediatR;

namespace FasTnT.Commands.Requests
{
    public class UnsubscribeRequest : IRequest<IEpcisResponse>
    {
        public string SubscriptionId { get; set; }
    }
}
