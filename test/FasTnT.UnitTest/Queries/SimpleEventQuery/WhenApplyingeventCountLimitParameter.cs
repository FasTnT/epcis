using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingeventCountLimitParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "eventCountLimit", Values = new[] { "5" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithLimitFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<LimitFilter>(f => f.Value == 5)), Times.Once);
        }
    }
}
