using FasTnT.Domain;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAGetVendorVersionRequest : BaseQueryServiceUnitTest
    {
        static string ExpectedVersion = Constants.ProductVersion;

        public GetVendorVersionResponse Response { get; set; }

        public override void Act() => Response = QueryService.GetVendorVersion().Result;

        [Assert]
        public void TheResponseShouldNotBeNull() => Assert.IsNotNull(Response);

        [Assert]
        public void TheResponseShouldContainTheSoftwareVersion() => Assert.AreEqual(ExpectedVersion, Response.Version);
    }
}
