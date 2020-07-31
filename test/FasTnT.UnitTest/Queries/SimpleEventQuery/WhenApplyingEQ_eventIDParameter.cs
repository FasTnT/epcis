using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_eventIDParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_eventID", Values = new[] { "test_ID" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSimpleParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<SimpleParameterFilter<string>>(f => f.Field == EpcisField.EventId && f.Values.Any(v => v.ToString() == "test_ID"))), Times.Once);
        }
    }
}
