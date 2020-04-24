using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;
using FasTnT.Model;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using static FasTnT.Commands.Requests.CaptureEpcisExceptionRequest;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingACaptureEpcisExceptionRequest : TestBase
    {
        public Mock<IDocumentStore> DocumentStore { get; set; }
        public RequestContext RequestContext { get; set; }
        public CaptureEpcisExceptionHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public CaptureEpcisExceptionRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            DocumentStore = new Mock<IDocumentStore>();
            CancellationToken = new CancellationTokenSource().Token;
            RequestContext = new RequestContext();
            Request = new CaptureEpcisExceptionRequest { Header = new EpcisRequest(), CallbackType = QueryCallbackType.ImplementationException, SubscriptionName = "test_sub" };
            Handler = new CaptureEpcisExceptionHandler(RequestContext, DocumentStore.Object);
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldCallTheDocumentStoreCaptureMethod()
        {
            DocumentStore.Verify(x => x.Capture(It.Is<CaptureCallbackRequest>(r => r.CallbackType == QueryCallbackType.ImplementationException && r.SubscriptionId == "test_sub"), It.IsAny<RequestContext>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void ItShouldReturnAnEmptyResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(EmptyResponse));
        }
    }
}
