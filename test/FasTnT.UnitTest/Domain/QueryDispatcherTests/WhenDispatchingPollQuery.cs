using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    [TestClass]
    public class WhenDispatchingPollQuery : QueryDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Query = new Poll { QueryName = "SimpleEventQuery" };
        }

        [Assert]
        public void TheResponseShouldNotBeNull()
        {
            Assert.IsNotNull(Response);
        }

        [Assert]
        public void TheResponseShouldBeAPollResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(PollResponse));
        }
    }
}
