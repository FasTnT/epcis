using FasTnT.Model.Queries;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.Queries
{
    [TestClass]
    public class WhenValidatingLE_ParameterWithNumericValueForSimpleEventQuery : SimpleEventQueryParameterValidationFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Parameters = new QueryParameter[]
            {
                new QueryParameter{ Name = "LE_eventTime", Values = new []{ "5.84" } }
            };
        }

        [Assert]
        public void ItShouldNotThrowAnException() => Assert.IsNull(Catched);
    }
}
