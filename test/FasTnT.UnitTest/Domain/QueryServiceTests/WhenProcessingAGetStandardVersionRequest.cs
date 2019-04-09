using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAGetStandardVersionRequest : BaseQueryServiceUnitTest
    {
        const string ExpectedVersion = "1.2";

        public GetStandardVersionResponse Response { get; set; }

        public override void Act() => Response = QueryService.GetStandardVersion().Result;

        [Assert]
        public void TheResponseShouldNotBeNull() => Assert.IsNotNull(Response);

        [Assert]
        public void TheResponseShouldContainTheVersion12() => Assert.AreEqual(ExpectedVersion, Response.Version);
    }
}
