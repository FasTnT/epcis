using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQATTR_Parameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQATTR_bizLocation_attributeName", Values = new[] { "some value" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonCustomFieldParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<AttributeFilter>(f => f.Field == EpcisField.BusinessLocation && f.AttributeName == "attributeName" && f.Values.Single() == "some value")), Times.Once);
        }
    }
}
