using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Handlers.Poll;
using FasTnT.Domain.Queries;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAPollRequestGivenQueryDoesNotExist : TestBase
    {
        public Mock<IEpcisQuery> Query { get; set; }
        public PollHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public PollRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }
        public Exception Catched { get; set; }

        public override void Given()
        {
            CancellationToken = new CancellationTokenSource().Token;
            Query = new Mock<IEpcisQuery>();
            Request = new PollRequest { QueryName = "UnknownQuery", Parameters = null };
            Handler = new PollHandler(new[] { Query.Object });

            Query.SetupGet(x => x.Name).Returns("ExampleQuery");
        }

        public override void When()
        {
            try
            {
                Response = Handler.Handle(Request, CancellationToken).Result;
            }
            catch(Exception ex)
            {
                Catched = ex?.InnerException ?? ex;
            }
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod]
        public void ItShouldThrowAnEpcisException()
        {
            Assert.IsInstanceOfType(Catched, typeof(EpcisException));
        }

        [TestMethod]
        public void ItShouldThrowANoSuchNameException()
        {
            var exception = (EpcisException)Catched;
            Assert.AreEqual(ExceptionType.NoSuchNameException, exception.ExceptionType);
        }

        [TestMethod]
        public void ItShouldNotCallTheIncorrectQueryHandleMethod()
        {
            Query.Verify(x => x.Handle(It.IsAny<QueryParameter[]>(), CancellationToken), Times.Never);
        }
    }
}
