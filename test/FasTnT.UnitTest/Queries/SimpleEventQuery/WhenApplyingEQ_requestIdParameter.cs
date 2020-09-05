using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_requestIdParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_requestId", Values = new[] { "6" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSimpleParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<SimpleParameterFilter<int>>(f => f.Field == EpcisField.RequestId && f.Values.Any(v => v == 6))), Times.Once);
        }
    }
}
