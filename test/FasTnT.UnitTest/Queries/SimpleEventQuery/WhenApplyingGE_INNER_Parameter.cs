using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingGE_INNER_Parameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "GE_INNER_namespace#name", Values = new[] { "15" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonCustomFieldParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<ComparisonCustomFieldFilter>(f => f.Field.Namespace == "namespace" && f.Field.Name == "name" && f.Comparator == FilterComparator.GreaterOrEqual && f.IsInner == true && f.Value.ToString() == "15")), Times.Once);
        }
    }
}
