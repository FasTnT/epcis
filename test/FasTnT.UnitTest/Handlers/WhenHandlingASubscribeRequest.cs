using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Handlers.Subscribe;
using FasTnT.Domain.Notifications;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingASubscribeRequest : TestBase
    {
        public Mock<IMediator> Mediator { get; set; }
        public SubscribeHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public SubscribeRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            CancellationToken = new CancellationTokenSource().Token;
            Request = new SubscribeRequest();
            Handler = new SubscribeHandler(Mediator.Object);
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
