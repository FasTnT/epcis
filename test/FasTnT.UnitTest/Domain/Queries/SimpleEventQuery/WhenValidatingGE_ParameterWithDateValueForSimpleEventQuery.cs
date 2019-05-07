using FasTnT.Model.Queries;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.Queries
{
    [TestClass]
    public class WhenValidatingGE_ParameterWithDateValueForSimpleEventQuery : SimpleEventQueryParameterValidationFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Parameters = new QueryParameter[]
            {
                new QueryParameter{ Name = "GE_eventTime", Values = new []{ "2019-05-01T10:02:54Z" } }
            };
        }

        [Assert]
        public void ItShouldNotThrowAnException() => Assert.IsNull(Catched);
    }
}
