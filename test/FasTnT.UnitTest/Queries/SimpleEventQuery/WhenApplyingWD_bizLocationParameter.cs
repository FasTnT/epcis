using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingWD_bizLocationParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "WD_bizLocation", Values = new[] { "loc_id" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMasterdataHierarchyFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MasterdataHierarchyFilter>(f => f.Field == EpcisField.BusinessLocation && f.Values.Any(v => v.ToString() == "loc_id"))), Times.Once);
        }
    }
}
