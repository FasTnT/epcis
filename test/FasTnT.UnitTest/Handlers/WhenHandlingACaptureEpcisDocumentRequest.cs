using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model;
using FasTnT.Domain.Handlers.CaptureEpcisDocument;
using FasTnT.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingACaptureEpcisDocumentRequest : TestBase
    {
        public Mock<IDocumentStore> DocumentStore { get; set; }
        public RequestContext RequestContext { get; set; }
        public CaptureEpcisDocumentHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public CaptureEpcisDocumentRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            DocumentStore = new Mock<IDocumentStore>();
            CancellationToken = new CancellationTokenSource().Token;
            RequestContext = new RequestContext();
            Request = new CaptureEpcisDocumentRequest { Header = new EpcisRequestHeader(), EventList = new List<EpcisEvent>() };
            Handler = new CaptureEpcisDocumentHandler(RequestContext, DocumentStore.Object);
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldCallTheDocumentStoreCaptureMethod()
        {
            DocumentStore.Verify(x => x.Capture(It.IsAny<CaptureDocumentRequest>(), It.IsAny<RequestContext>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void ItShouldReturnAnEmptyResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(EmptyResponse));
        }
    }
}
