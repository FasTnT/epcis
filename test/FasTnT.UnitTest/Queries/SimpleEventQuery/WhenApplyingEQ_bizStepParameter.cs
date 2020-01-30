using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Events.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_bizStepParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_bizStep", Values = new[] { "urn:fastnt:demo:bizstep:demo:packing" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<SimpleParameterFilter<string>>(f => f.Field == EpcisField.BusinessStep&& f.Values.Any(v => v.ToString() == "urn:fastnt:demo:bizstep:demo:packing"))), Times.Once);
        }
    }
}
