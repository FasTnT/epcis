using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingGT_quantityParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "GT_quantity", Values = new[] { "37" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSourceDestinationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<QuantityFilter>(f => f.Operator == FilterComparator.GreaterThan && f.Value == 37)), Times.Once);
        }
    }
}
