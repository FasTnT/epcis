using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_INNER_Parameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_INNER_namespace#name", Values = new[] { "DEA" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonCustomFieldParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<CustomFieldFilter>(f => f.Field.Namespace == "namespace" && f.Field.Name == "name" && f.IsInner == true && f.Values.Single().ToString() == "DEA")), Times.Once);
        }
    }
}
