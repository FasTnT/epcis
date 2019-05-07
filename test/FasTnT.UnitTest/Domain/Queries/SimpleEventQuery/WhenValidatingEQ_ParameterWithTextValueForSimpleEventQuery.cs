using FasTnT.Model.Queries;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.Queries
{
    [TestClass]
    public class WhenValidatingEQ_ParameterWithTextValueForSimpleEventQuery : SimpleEventQueryParameterValidationFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Parameters = new QueryParameter[]
            {
                new QueryParameter{ Name = "EQ_eventTime", Values = new []{ "some text value" } }
            };
        }

        [Assert]
        public void ItShouldNowThrowAnException() => Assert.IsNull(Catched);
    }
}
