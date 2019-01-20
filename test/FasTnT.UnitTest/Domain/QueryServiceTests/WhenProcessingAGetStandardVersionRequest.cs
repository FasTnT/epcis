using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAGetStandardVersionRequest : BaseQueryServiceUnitTest
    {
        const string ExpectedVersion = "1.2";

        public GetStandardVersion Request { get; set; }
        public GetStandardVersionResponse Response { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new GetStandardVersion();
        }

        public override void Act() => Response = QueryService.Process(Request).Result;

        [Assert]
        public void TheResponseShouldNotBeNull() => Assert.IsNotNull(Response);

        [Assert]
        public void TheResponseShouldContainTheVersion12() => Assert.AreEqual(ExpectedVersion, Response.Version);
    }
}
