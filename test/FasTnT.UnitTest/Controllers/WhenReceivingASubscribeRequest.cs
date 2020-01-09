using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using FasTnT.Host.Controllers.v1_2;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace FasTnT.UnitTest.Controllers
{
    [TestClass]
    public class WhenReceivingASubscribeRequest : TestBase
    {
        public EpcisQueryController Controller { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public IQueryRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            Request = new SubscribeRequest();
            CancellationToken = new CancellationTokenSource().Token;
            Controller = new EpcisXmlQueryController(Mediator.Object);
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
        public void ItShouldReturnANullObject()
        {
            Assert.IsNull(Response);
        }
    }
}
