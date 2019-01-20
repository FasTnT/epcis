using FakeItEasy;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAPollRequestForSimpleEventQuery : BaseQueryServiceUnitTest
    {
        public Poll Request { get; set; }
        public PollResponse Response { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new Poll { QueryName = "SimpleEventQuery", Parameters = new QueryParameter[0] };
        }

        public override void Act() => Response = QueryService.Process(Request).Result;

        [Assert]
        public void TheResponseShouldNotBeNull() => Assert.IsNotNull(Response);

        [Assert]
        public void ItShouldHaveCalledTheSubscriptionManagerGetAll() => A.CallTo(() => UnitOfWork.EventManager).MustHaveHappened();
    }
}
