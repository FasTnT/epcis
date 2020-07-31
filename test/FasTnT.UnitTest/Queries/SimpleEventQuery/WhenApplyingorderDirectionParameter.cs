using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingorderDirectionParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "orderDirection", Values = new[] { "ASC" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithOrderDirectionFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<OrderDirectionFilter>(f => f.Direction == OrderDirection.Ascending)), Times.Once);
        }
    }
}
