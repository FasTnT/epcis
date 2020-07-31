using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingmaxEventCountParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "maxEventCount", Values = new[] { "5" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithLimitFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<LimitFilter>(f => f.Value == 6)), Times.Once);
        }
    }
}
