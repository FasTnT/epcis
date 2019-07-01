using FasTnT.Model.Queries;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    [TestClass]
    public class WhenDispatchingPollQueryForUnknownQuery : QueryDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Query = new Poll { QueryName = "UnknownQueryName" };
        }

        [Assert]
        public void TheResponseShouldNotBeNull()
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
