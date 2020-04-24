using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_actionParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_action", Values = new[] { "ADD" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<SimpleParameterFilter<EventAction>>(f => f.Field == EpcisField.Action && f.Values.Any(v => v.ToString() == "ADD"))), Times.Once);
        }
    }
}
