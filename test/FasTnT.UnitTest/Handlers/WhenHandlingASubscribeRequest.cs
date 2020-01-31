using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Notifications;
using FasTnT.Domain.Queries;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using static FasTnT.Commands.Requests.SubscribeRequest;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingASubscribeRequest : TestBase
    {
        public Mock<IMediator> Mediator { get; set; }
        public Mock<IEpcisQuery> Query { get; set; }
        public Mock<ISubscriptionManager> SubscriptionManager { get; set; }
        public SubscribeHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public SubscribeRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            Query = new Mock<IEpcisQuery>();
            SubscriptionManager = new Mock<ISubscriptionManager>();
            CancellationToken = new CancellationTokenSource().Token;
            Request = new SubscribeRequest { Subscription = new Domain.Model.Subscriptions.Subscription{ QueryName = "TestQuery" } };
            Handler = new SubscribeHandler(new[] { Query.Object }, SubscriptionManager.Object, Mediator.Object);

            Query.SetupGet(x => x.Name).Returns("TestQuery");
            Query.SetupGet(x => x.AllowSubscription).Returns(true);
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldPublishASubscriptionCreatedNotification()
        {
            Mediator.Verify(x => x.Publish(It.IsAny<SubscriptionCreatedNotification>(), CancellationToken), Times.Once);
        }
    }
}
