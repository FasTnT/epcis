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
using System.Data;
using System.Threading;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingACaptureEpcisDocumentRequest : TestBase
    {
        public Mock<ITransactionProvider> TransactionProvider { get; set; }
        public Mock<IDocumentStore> DocumentStore { get; set; }
        public Mock<IDbTransaction> Transaction { get; set; }
        public RequestContext RequestContext { get; set; }
        public CaptureEpcisDocumentHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public CaptureEpcisDocumentRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            Transaction = new Mock<IDbTransaction>();
            TransactionProvider = new Mock<ITransactionProvider>();
            DocumentStore = new Mock<IDocumentStore>();
            CancellationToken = new CancellationTokenSource().Token;
            RequestContext = new RequestContext();
            Request = new CaptureEpcisDocumentRequest { Header = new EpcisRequestHeader(), EventList = new List<EpcisEvent>() };
            Handler = new CaptureEpcisDocumentHandler(TransactionProvider.Object, RequestContext, DocumentStore.Object);

            TransactionProvider.Setup(x => x.BeginTransaction()).Returns(Transaction.Object);
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldCallTheTransactionProviderBeginTransactionMethod()
        {
            TransactionProvider.Verify(x => x.BeginTransaction(), Times.Once);
        }

        [TestMethod]
        public void ItShouldCallTheDocumentStoreCaptureMethod()
        {
            DocumentStore.Verify(x => x.Capture(It.IsAny<CaptureDocumentRequest>()), Times.Once);
        }

        [TestMethod]
        public void ItShouldCallTheTransactionCommitMethod()
        {
            Transaction.Verify(x => x.Commit(), Times.Once);
        }

        [TestMethod]
        public void ItShouldReturnAnEmptyResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(EmptyResponse));
        }
    }
}
