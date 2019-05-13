using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FasTnT.UnitTest.Domain.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAGetQueryNamesRequest : BaseQueryServiceUnitTest
    {
        public GetQueryNamesResponse Response { get; set; }

        public override void Act() => Response = QueryService.GetQueryNames(default).Result;

        [Assert]
        public void TheResponseShouldNotBeNull() => Assert.IsNotNull(Response);

        [Assert]
        public void TheResponseShouldContainAListOfQueryNames() => Assert.IsNotNull(Response.QueryNames);

        [Assert]
        public void TheResponseShouldContainTheNameOfTheQueriesInjectedInTheService() => CollectionAssert.AreEquivalent(EpcisQueries.Select(x => x.Name).ToArray(), Response.QueryNames.ToArray());
    }
}
