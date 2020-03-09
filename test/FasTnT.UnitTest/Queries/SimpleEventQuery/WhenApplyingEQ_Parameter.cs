using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_Parameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_namespace#name", Values = new[] { "1-5" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonCustomFieldParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<CustomFieldFilter>(f => f.Field.Namespace == "namespace" && f.Field.Name == "name" && f.IsInner == false && f.Values.Single().ToString() == "1-5")), Times.Once);
        }
    }
}
