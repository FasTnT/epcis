using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Events;
using FasTnT.Model.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingmaxEventCountParameterAndMoreEventsAreReturned : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            EventFetcher.Setup(x => x.Fetch(It.IsAny<CancellationToken>())).Returns(() => Task.FromResult<IEnumerable<EpcisEvent>>(new[] { new EpcisEvent(), new EpcisEvent() }));
            Parameters.Add(new Model.Queries.QueryParameter { Name = "maxEventCount", Values = new[] { "1" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithLimitFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<LimitFilter>(f => f.Value == 2)), Times.Once);
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
            Assert.IsInstanceOfType(Catched, typeof(EpcisException));
            Assert.AreEqual(ExceptionType.QueryTooLargeException, ((EpcisException)Catched).ExceptionType);
        }
    }
}
