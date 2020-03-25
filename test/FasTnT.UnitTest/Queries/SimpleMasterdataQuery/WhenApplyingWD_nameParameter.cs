using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    [TestClass]
    public class WhenApplyingWD_nameParameter : SimpleMasterdataQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "WD_name", Values = new[] { "masterdata-dname" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonParameterFilter()
        {
            MasterdataFetcher.Verify(x => x.Apply(It.Is<MasterdataDescendentNameFilter>(f => f.Values.Contains("masterdata-dname"))), Times.Once);
        }
    }
}
