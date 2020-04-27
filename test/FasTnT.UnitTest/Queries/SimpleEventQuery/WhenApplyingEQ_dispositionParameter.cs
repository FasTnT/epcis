using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_dispositionParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_disposition", Values = new[] { "urn:epcglobal:cbv:disp:loading" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<SimpleParameterFilter<string>>(f => f.Field == EpcisField.Disposition && f.Values.Any(v => v.ToString() == "urn:epcglobal:cbv:disp:loading"))), Times.Once);
        }
    }
}
