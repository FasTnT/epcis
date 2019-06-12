using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    [TestClass]
    public class WhenDispatchingGetQueryNamesQuery : QueryDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Query = new GetQueryNames();
        }

        [Assert]
        public void TheResponseShouldNotBeNull()
        {
            Assert.IsNotNull(Response);
        }

        [Assert]
        public void TheResponseShouldBeAGetQueryNamesResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(GetQueryNamesResponse));
        }
    }
}
