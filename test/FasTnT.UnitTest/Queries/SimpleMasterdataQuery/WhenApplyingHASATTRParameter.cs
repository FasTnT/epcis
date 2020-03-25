using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    [TestClass]
    public class WhenApplyingHASATTRParameter : SimpleMasterdataQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "HASATTR", Values = new[] { "masterdata-attribute-name" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonParameterFilter()
        {
            MasterdataFetcher.Verify(x => x.Apply(It.Is<MasterdataExistsAttibuteFilter>(f => f.Values.Contains("masterdata-attribute-name"))), Times.Once);
        }
    }
}
