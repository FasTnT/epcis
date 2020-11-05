using FasTnT.Commands.Responses;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Subscriptions
{
    [TestClass]
    public class WhenRunningSubscriptionWithReportWhenEmptyTrueAndNoPendingRequest : SubscriptionRunnerTests
    {
        public override void Given()
        {
            base.Given();

            ResultSender.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<IEpcisResponse>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(true));
            Subscription = new Subscription { SubscriptionId = "sub", QueryName = "SimpleEventQuery", Destination = "http://subscription-result.com/callback", ReportIfEmpty = true };
        }

        [TestMethod]
        public void ItShouldSendTheResultsToTheDestination()
        {
            ResultSender.Verify(x => x.Send("http://subscription-result.com/callback", It.IsAny<PollResponse>(), default), Times.Once);
        }

        [TestMethod]
        public void ItShouldStoreAnError()
        {
            SubscriptionManager.Verify(x => x.RegisterSubscriptionTrigger("sub", SubscriptionResult.Success, default, default), Times.Once);
        }
    }
}
