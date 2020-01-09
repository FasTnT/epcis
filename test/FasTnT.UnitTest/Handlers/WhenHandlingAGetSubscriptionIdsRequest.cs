using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Handlers.GetSubscriptionIds;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAGetSubscriptionIdsRequest : TestBase
    {
        public string[] SubscriptionIds { get; set; }
        public Mock<ISubscriptionManager> SubscriptionManager { get; set; }
        public GetSubscriptionIdsHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public GetSubscriptionIdsRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            SubscriptionIds = new[] { "Sub1", "Sub2" };
            CancellationToken = new CancellationTokenSource().Token;
            SubscriptionManager = new Mock<ISubscriptionManager>();
            Request = new GetSubscriptionIdsRequest();
            Handler = new GetSubscriptionIdsHandler(SubscriptionManager.Object);

            SubscriptionManager.Setup(x => x.GetSubscriptionIds()).Returns(() => Task.FromResult(SubscriptionIds));
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldCallTheSubscriptionManagerGetSubscriptionIdsMethod()
        {
            SubscriptionManager.Verify(x => x.GetSubscriptionIds(), Times.Once);
        }

        [TestMethod]
        public void ItShouldReturnAGetSubscriptionIdsResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(GetSubscriptionIdsResponse));
        }

        [TestMethod]
        public void ItShouldReturnTheListOfSubscriptionIds()
        {
            CollectionAssert.AreEquivalent(SubscriptionIds, ((GetSubscriptionIdsResponse)Response).SubscriptionIds);
        }
    }
}
