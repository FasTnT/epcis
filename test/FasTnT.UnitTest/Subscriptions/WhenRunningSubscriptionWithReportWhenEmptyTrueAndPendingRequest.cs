using FasTnT.Commands.Responses;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Subscriptions
{
    [TestClass]
    public class WhenRunningSubscriptionWithReportWhenEmptyTrueAndPendingRequest : SubscriptionRunnerTests
    {
        public override void Given()
        {
            base.Given();

            ResultSender.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<IEpcisResponse>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(true));
            SubscriptionManager.Setup(x => x.GetPendingRequestIds("sub", default)).Returns(() => Task.FromResult(new[] { 1, 4 }));
            Subscription = new Subscription { SubscriptionId = "sub", QueryName = "SimpleEventQuery", Destination = "http://subscription-result.com/callback", ReportIfEmpty = true };
        }

        [TestMethod]
        public void ItShouldSendTheResultsToTheDestination()
        {
            ResultSender.Verify(x => x.Send("http://subscription-result.com/callback", It.IsAny<PollResponse>(), default), Times.Once);
        }

        [TestMethod]
        public void ItShouldRegisterSubscriptionTrigger()
        {
            SubscriptionManager.Verify(x => x.RegisterSubscriptionTrigger("sub", SubscriptionResult.Success, default, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void ItShouldAcknowledgePendingRequests()
        {
            SubscriptionManager.Verify(x => x.AcknowledgePendingRequests("sub", new[] { 1, 4 }, default), Times.Once);
        }
    }
}
