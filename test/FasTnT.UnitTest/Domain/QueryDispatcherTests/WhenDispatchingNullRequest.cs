using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    [TestClass]
    public class WhenDispatchingNullRequest : QueryDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Query = default;
        }

        [Assert]
        public void TheResponseShouldBeNull()
        {
            Assert.IsNull(Response);
        }

        [Assert]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Exception);
        }
    }
}
