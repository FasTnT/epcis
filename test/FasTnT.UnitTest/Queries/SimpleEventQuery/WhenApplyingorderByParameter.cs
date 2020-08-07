using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingorderByParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "orderBy", Values = new[] { "bizLocation" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithOrderFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<OrderFilter>(f => f.Field == EpcisField.BusinessLocation)), Times.Once);
        }
    }
}
