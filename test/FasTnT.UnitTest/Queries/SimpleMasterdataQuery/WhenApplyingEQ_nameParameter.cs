using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    [TestClass]
    public class WhenApplyingEQ_nameParameter : SimpleMasterdataQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_name", Values = new[] { "masterdata-name" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithComparisonParameterFilter()
        {
            MasterdataFetcher.Verify(x => x.Apply(It.Is<MasterdataNameFilter>(f => f.Values.Contains("masterdata-name"))), Times.Once);
        }
    }
}
