using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Handlers.Unsubscribe;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Domain.Notifications;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAnUnsubscribeRequest : TestBase
    {
        public Mock<IMediator> Mediator { get; set; }
        public Mock<ISubscriptionManager> SubscriptionManager { get; set; }
        public UnsubscribeHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public UnsubscribeRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            SubscriptionManager = new Mock<ISubscriptionManager>();
            CancellationToken = new CancellationTokenSource().Token;
            Request = new UnsubscribeRequest { SubscriptionId = "test-subscription" };
            Handler = new UnsubscribeHandler(SubscriptionManager.Object, Mediator.Object);

            SubscriptionManager.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new Subscription()));
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldPublishASubscriptionRemovedNotification()
        {
            Mediator.Verify(x => x.Publish(It.IsAny<SubscriptionRemovedNotification>(), CancellationToken), Times.Once);
        }
    }
}
