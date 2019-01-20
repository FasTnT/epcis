using FakeItEasy;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FasTnT.UnitTest.Domain.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAGetSubscriptionIDsRequest : BaseQueryServiceUnitTest
    {
        public GetSubscriptionIds Request { get; set; }
        public GetSubscriptionIdsResult Response { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new GetSubscriptionIds { QueryName = EpcisQueries.First().Name };
        }

        public override void Act() => Response = QueryService.Process(Request).Result;

        [Assert]
        public void TheResponseShouldNotBeNull() => Assert.IsNotNull(Response);

        [Assert]
        public void TheResponseShouldContainAListOfSubscriptionIds() => Assert.IsNotNull(Response.SubscriptionIds);

        [Assert]
        public void ItShouldHaveCalledTheSubscriptionManagerGetAll() => A.CallTo(() => UnitOfWork.SubscriptionManager).MustHaveHappened();
    }
}
