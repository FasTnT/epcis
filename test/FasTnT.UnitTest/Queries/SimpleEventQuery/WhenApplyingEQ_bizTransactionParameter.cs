using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_bizTransactionParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_bizTransaction_bizTransType", Values = new[] { "test_biztrans" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSourceDestinationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<BusinessTransactionFilter>(f => f.TransactionType == "bizTransType" && f.Values.Any(v => v.ToString() == "test_biztrans"))), Times.Once);
        }
    }
}
