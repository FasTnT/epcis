using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using FasTnT.Host.Controllers.v1_2;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Controllers
{
    [TestClass]
    public class WhenReceivingAGetSubscriptionIdsRequest : TestBase
    {
        public EpcisSoapQueryController Controller { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public IQueryRequest Request { get; set; }
        public IEpcisResponse ExpectedResponse { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            ExpectedResponse = new GetSubscriptionIdsResponse();
            Mediator = new Mock<IMediator>();
            Request = new GetSubscriptionIdsRequest();
            CancellationToken = new CancellationTokenSource().Token;
            Controller = new EpcisSoapQueryController(Mediator.Object);

            Mediator.Setup(x => x.Send(Request, CancellationToken)).Returns(() => Task.FromResult(ExpectedResponse));
        }

        public override void When()
        {
            Response = Controller.Query(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldSendTheCommandToTheMediator()
        {
            Mediator.Verify(x => x.Send(Request, CancellationToken), Times.Once);
        }

        [TestMethod]
        public void ItShouldReturnTheResponseFromMediator()
        {
            Assert.AreEqual(Response, ExpectedResponse);
        }
    }
}
