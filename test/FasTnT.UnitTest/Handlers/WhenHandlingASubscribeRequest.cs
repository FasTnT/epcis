using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Handlers.Subscribe;
using FasTnT.Domain.Notifications;
using FasTnT.Domain.Queries;
using FasTnT.Model.Exceptions;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingASubscribeRequest : TestBase
    {
        public Mock<IMediator> Mediator { get; set; }
        public Mock<IEpcisQuery> Query { get; set; }
        public SubscribeHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public SubscribeRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            Query = new Mock<IEpcisQuery>();
            CancellationToken = new CancellationTokenSource().Token;
            Request = new SubscribeRequest { Subscription = new Domain.Model.Subscriptions.Subscription{ QueryName = "TestQuery" } };
            Handler = new SubscribeHandler(new[] { Query.Object }, null, Mediator.Object); // TODO

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

    [TestClass]
    public class WhenHandlingASubscribeRequestGivenTheQueryDoesNotExist : TestBase
    {
        public Mock<IMediator> Mediator { get; set; }
        public Mock<IEpcisQuery> Query { get; set; }
        public SubscribeHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public SubscribeRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }
        public Exception Catched { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            Query = new Mock<IEpcisQuery>();
            CancellationToken = new CancellationTokenSource().Token;
            Request = new SubscribeRequest { Subscription = new Domain.Model.Subscriptions.Subscription { QueryName = "UnknownQuery" } };
            Handler = new SubscribeHandler(new[] { Query.Object }, null, Mediator.Object); // TODO

            Query.SetupGet(x => x.Name).Returns("TestQuery");
        }

        public override void When()
        {
            try
            {
                Response = Handler.Handle(Request, CancellationToken).Result;
            }
            catch(Exception ex)
            {
                Catched = ex?.InnerException ?? ex;
            }
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod]
        public void ItShouldThrowAnEpcisException()
        {
            Assert.IsInstanceOfType(Catched, typeof(EpcisException));
        }

        [TestMethod]
        public void ItShouldNotPublishASubscriptionCreatedNotification()
        {
            Mediator.Verify(x => x.Publish(It.IsAny<SubscriptionCreatedNotification>(), CancellationToken), Times.Never);
        }
    }

    [TestClass]
    public class WhenHandlingASubscribeRequestGivenTheQueryDoesNotAllowSubscription : TestBase
    {
        public Mock<IMediator> Mediator { get; set; }
        public Mock<IEpcisQuery> Query { get; set; }
        public SubscribeHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public SubscribeRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }
        public Exception Catched { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            Query = new Mock<IEpcisQuery>();
            CancellationToken = new CancellationTokenSource().Token;
            Request = new SubscribeRequest { Subscription = new Domain.Model.Subscriptions.Subscription { QueryName = "TestQuery" } };
            Handler = new SubscribeHandler(new[] { Query.Object }, null, Mediator.Object); // TODO

            Query.SetupGet(x => x.Name).Returns("TestQuery");
        }

        public override void When()
        {
            try
            {
                Response = Handler.Handle(Request, CancellationToken).Result;
            }
            catch (Exception ex)
            {
                Catched = ex?.InnerException ?? ex;
            }
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod]
        public void ItShouldThrowAnEpcisException()
        {
            Assert.IsInstanceOfType(Catched, typeof(EpcisException));
        }

        [TestMethod]
        public void ItShouldNotPublishASubscriptionCreatedNotification()
        {
            Mediator.Verify(x => x.Publish(It.IsAny<SubscriptionCreatedNotification>(), CancellationToken), Times.Never);
        }
    }
}
