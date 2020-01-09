using FasTnT.Commands.Requests;
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
    public class WhenCapturingADocument : TestBase
    {
        public EpcisCaptureController Controller { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public ICaptureRequest Request { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            Request = new CaptureEpcisDocumentRequest();
            CancellationToken = new CancellationTokenSource().Token;
            Controller = new EpcisCaptureController(Mediator.Object);
        }

        public override void When()
        {
            Task.WaitAll(Controller.Capture(Request, CancellationToken));
        }

        [TestMethod]
        public void ItShouldSendTheCommandToTheMediator()
        {
            Mediator.Verify(x => x.Send(Request, CancellationToken), Times.Once);
        }
    }
}
