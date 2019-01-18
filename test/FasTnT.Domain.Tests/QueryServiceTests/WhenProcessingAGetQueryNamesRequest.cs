using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FasTnT.Domain.Tests.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAGetQueryNamesRequest : BaseQueryServiceUnitTest
    {
        public GetQueryNames Request { get; set; }
        public GetQueryNamesResponse Response { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new GetQueryNames();
        }

        public override void Act() => Response = QueryService.Process(Request).Result;

        [Assert]
        public void TheResponseShouldNotBeNull() => Assert.IsNotNull(Response);

        [Assert]
        public void TheResponseShouldContainAListOfQueryNames() => Assert.IsNotNull(Response.QueryNames);

        [Assert]
        public void TheResponseShouldContainTheNameOfTheQueriesInjectedInTheService() => CollectionAssert.AreEquivalent(EpcisQueries.Select(x => x.Name).ToArray(), Response.QueryNames.ToArray());
    }
}
