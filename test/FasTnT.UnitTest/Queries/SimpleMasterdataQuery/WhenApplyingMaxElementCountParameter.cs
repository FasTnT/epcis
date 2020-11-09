using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    [TestClass]
    public class WhenApplyingMaxElementCountParameter : SimpleMasterdataQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "maxElementCount", Values = new[] { "10" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithLimitFilter()
        {
            MasterdataFetcher.Verify(x => x.Apply(It.Is<LimitFilter>(f => f.Value == 11)), Times.Once);
        }
    }
}
