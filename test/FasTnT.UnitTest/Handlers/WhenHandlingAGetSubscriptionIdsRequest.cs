using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static FasTnT.Commands.Requests.GetSubscriptionIdsRequest;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAGetSubscriptionIdsRequest : TestBase
    {
        public IEnumerable<Subscription> Subscriptions { get; set; }
        public Mock<ISubscriptionManager> SubscriptionManager { get; set; }
        public GetSubscriptionIdsHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public GetSubscriptionIdsRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            Subscriptions = new[] { new Subscription { SubscriptionId = "Sub1" }, new Subscription { SubscriptionId = "Sub2" } };
            CancellationToken = new CancellationTokenSource().Token;
            SubscriptionManager = new Mock<ISubscriptionManager>();
            Request = new GetSubscriptionIdsRequest { QueryName = "TestQuery" };
            Handler = new GetSubscriptionIdsHandler(SubscriptionManager.Object);

            SubscriptionManager.Setup(x => x.GetAll(It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(Subscriptions));
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldCallTheSubscriptionManagerGetSubscriptionIdsMethod()
        {
            SubscriptionManager.Verify(x => x.GetAll(CancellationToken), Times.Once);
        }

        [TestMethod]
        public void ItShouldReturnAGetSubscriptionIdsResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(GetSubscriptionIdsResponse));
        }

        [TestMethod]
        public void ItShouldReturnTheListOfSubscriptionIds()
        {
            CollectionAssert.AreEquivalent(new[] { "Sub1", "Sub2" }, ((GetSubscriptionIdsResponse)Response).SubscriptionIds);
        }
    }
}
