using MediatR;

namespace FasTnT.Domain.Commands.Requests
{
    public class TriggerSubscriptionRequest : INotification
    {
        public string Name { get; set; }
    }
}
