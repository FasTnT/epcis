using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Handlers.GetQueryNames;
using FasTnT.Domain.Queries;
using FasTnT.Model.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAGetQueryNamesRequest : TestBase
    {
        public Mock<IEpcisQuery> Query { get; set; }
        public GetQueryNamesHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public GetQueryNamesRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            CancellationToken = new CancellationTokenSource().Token;
            Query = new Mock<IEpcisQuery>();
            Request = new GetQueryNamesRequest ();
            Handler = new GetQueryNamesHandler(new[] { Query.Object });

            Query.SetupGet(x => x.Name).Returns("ExampleQuery");
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldNotCallTheQueryHandleMethod()
        {
            Query.Verify(x => x.Handle(It.IsAny<QueryParameter[]>(), CancellationToken), Times.Never);
        }

        [TestMethod]
        public void ItShouldReturnAGetQueryNamesResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(GetQueryNamesResponse));
        }

        [TestMethod]
        public void TheResponseShouldContainTheQueryNames()
        {
            CollectionAssert.Contains(((GetQueryNamesResponse)Response).QueryNames, Query.Object.Name);
        }
    }
}
