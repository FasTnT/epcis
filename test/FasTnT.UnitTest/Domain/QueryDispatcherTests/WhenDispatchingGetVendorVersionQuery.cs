using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    [TestClass]
    public class WhenDispatchingGetVendorVersionQuery : QueryDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Query = new GetVendorVersion();
        }

        [Assert]
        public void TheResponseShouldNotBeNull()
        {
            Assert.IsNotNull(Response);
        }

        [Assert]
        public void TheResponseShouldBeAGetVendorVersionResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(GetVendorVersionResponse));
        }
    }
}
