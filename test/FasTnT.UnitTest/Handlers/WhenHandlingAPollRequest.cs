using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Queries;
using FasTnT.Model.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using static FasTnT.Commands.Requests.PollRequest;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAPollRequest : TestBase
    {
        public Mock<IEpcisQuery> Query { get; set; }
        public PollHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public PollRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            CancellationToken = new CancellationTokenSource().Token;
            Query = new Mock<IEpcisQuery>();
            Request = new PollRequest { QueryName = "ExampleQuery", Parameters = null };
            Handler = new PollHandler(new [] { Query.Object });

            Query.SetupGet(x => x.Name).Returns("ExampleQuery");
            Query.Setup(x => x.Handle(It.IsAny<QueryParameter[]>(), CancellationToken)).Returns(Task.FromResult(new PollResponse()));
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldCallTheCorrectQueryHandleMethod()
        {
            Query.Verify(x => x.Handle(It.IsAny<QueryParameter[]>(), CancellationToken), Times.Once);
        }

        [TestMethod]
        public void ItShouldReturnAPollResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(PollResponse));
        }
    }
}
