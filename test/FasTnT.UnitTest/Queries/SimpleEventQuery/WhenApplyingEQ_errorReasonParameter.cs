using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_errorReasonParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_errorReason", Values = new[] { "reason" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithEqualsErrorReasonFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<EqualsErrorReasonFilter>(f => f.Values.Any(v => v.ToString() == "reason"))), Times.Once);
        }
    }
}
