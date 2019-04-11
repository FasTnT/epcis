using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FasTnT.UnitTest.Domain.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAPollRequestForSimpleEventQueryWithUnknownParameter : BaseQueryServiceUnitTest
    {
        public Poll Request { get; set; }
        public PollResponse Response { get; set; }
        public Exception Catched { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new Poll { QueryName = "SimpleEventQuery", Parameters = new [] { new QueryParameter { Name = "UnknownParameter", Values = new[] { "value" } } } };
        }

        public override void Act()
        {
            try
            {
                Response = QueryService.Poll(Request, default).Result;
            }
            catch(Exception ex)
            {
                Catched = ex;
            }
        }

        [Assert]
        public void TheResponseShouldBeNull() => Assert.IsNull(Response);

        [Assert]
        public void ItShouldThrowAnException() => Assert.IsNotNull(Catched);
    }
}
