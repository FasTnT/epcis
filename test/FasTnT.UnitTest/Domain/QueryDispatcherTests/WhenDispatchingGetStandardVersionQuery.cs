using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    [TestClass]
    public class WhenDispatchingGetStandardVersionQuery : QueryDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Query = new GetStandardVersion();
        }

        [Assert]
        public void TheResponseShouldNotBeNull()
        {
            Assert.IsNotNull(Response);
        }

        [Assert]
        public void TheResponseShouldBeAGetStandardVersionResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(GetStandardVersionResponse));
        }
    }
}
