using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Events.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingLT_Parameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "LT_namespace#name", Values = new[] { "15" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonCustomFieldParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<ComparisonCustomFieldFilter>(f => f.Field.Namespace == "namespace" && f.Field.Name == "name" && f.Comparator == FilterComparator.LessThan && f.IsInner == false && f.Value.ToString() == "15")), Times.Once);
        }
    }
}
