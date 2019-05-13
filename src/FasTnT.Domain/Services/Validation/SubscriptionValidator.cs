using FasTnT.Domain.Services.Subscriptions;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Subscriptions;

namespace FasTnT.Domain.Services
{
    public static class SubscriptionValidator
    {
        public static void Validate(Subscription subscription)
        {
            EnsureDestinationIsValidURI(subscription);
            EnsureDestinationHasEndSlash(subscription);

            if (!(subscription.Schedule == null ^ string.IsNullOrEmpty(subscription.Trigger)))
            {
                throw new EpcisException(ExceptionType.SubscriptionControlsException, "Only one of the schedule and trigger must be provided");
            }
            if (!SubscriptionSchedule.IsValid(subscription))
            {
                throw new EpcisException(ExceptionType.SubscriptionControlsException, "Provided schedule parameters are invalid");
            }
        }

        private static void EnsureDestinationHasEndSlash(Subscription request) => request.Destination = $"{request.Destination.TrimEnd('/')}/";
        private static void EnsureDestinationIsValidURI(Subscription request) => UriValidator.Validate(request.Destination, true);
    }
}
