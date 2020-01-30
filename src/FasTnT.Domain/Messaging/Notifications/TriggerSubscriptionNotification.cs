using MediatR;

namespace FasTnT.Domain.Commands.Requests
{
    public class TriggerSubscriptionNotification : INotification
    {
        public string Name { get; set; }
    }
}
