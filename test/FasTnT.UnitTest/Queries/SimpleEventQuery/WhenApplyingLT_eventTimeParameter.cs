using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingLT_eventTimeParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "LT_eventTime", Values = new[] { "2018-04-01T10:21:10.521Z" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<ComparisonParameterFilter>(f => f.Comparator == FilterComparator.LessThan && f.Field == EpcisField.EventTime && f.Value is DateTime)), Times.Once);
        }
    }
}
