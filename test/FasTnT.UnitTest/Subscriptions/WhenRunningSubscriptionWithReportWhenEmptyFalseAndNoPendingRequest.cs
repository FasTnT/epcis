using FasTnT.Commands.Responses;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace FasTnT.UnitTest.Subscriptions
{
    [TestClass]
    public class WhenRunningSubscriptionWithReportWhenEmptyFalseAndNoPendingRequest : SubscriptionRunnerTests
    {
        public override void Given()
        {
            base.Given();

            Subscription = new Subscription { SubscriptionId = "sub", QueryName = "SimpleEventQuery", Destination = "http://subscription-result.com/callback", ReportIfEmpty = false };
        }

        [TestMethod]
        public void ItShouldNotSendTheResultsToTheDestination()
        {
            ResultSender.Verify(x => x.Send("http://subscription-result.com/callback", It.IsAny<PollResponse>(), default), Times.Never);
        }

        [TestMethod]
        public void ItShouldRegisterSubscriptionTrigger()
        {
            SubscriptionManager.Verify(x => x.RegisterSubscriptionTrigger("sub", SubscriptionResult.Success, default, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
