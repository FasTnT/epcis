using FasTnT.Model.Queries;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.Queries
{
    [TestClass]
    public class WhenValidatingeventTypeParameterForSimpleEventQuery : SimpleEventQueryParameterValidationFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Parameters = new QueryParameter[]
            {
                new QueryParameter{ Name = "eventType", Values = new []{ "test" } }
            };
        }

        [Assert]
        public void ItShouldNotThrowAnException() => Assert.IsNull(Catched);
    }
}
