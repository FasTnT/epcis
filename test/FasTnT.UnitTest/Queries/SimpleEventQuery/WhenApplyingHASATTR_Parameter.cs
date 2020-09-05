using FasTnT.Domain.Data.Model.Filters;
<<<<<<< HEAD
using FasTnT.Model.Events.Enums;
=======
using FasTnT.Model.Enums;
>>>>>>> develop
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingHASATTR_Parameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "HASATTR_bizLocation_attributeName", Values = null });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonCustomFieldParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<ExistsAttributeFilter>(f => f.Field == EpcisField.BusinessLocation && f.AttributeName == "attributeName")), Times.Once);
        }
    }
}
