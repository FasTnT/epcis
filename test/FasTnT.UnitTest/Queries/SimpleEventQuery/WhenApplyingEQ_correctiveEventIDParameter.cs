using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_correctiveEventIDParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_correctiveEventID", Values = new[] { "evt_id" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithEqualsCorrectiveEventIdFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<EqualsCorrectiveEventIdFilter>(f => f.Values.Any(v => v.ToString() == "evt_id"))), Times.Once);
        }
    }
}
