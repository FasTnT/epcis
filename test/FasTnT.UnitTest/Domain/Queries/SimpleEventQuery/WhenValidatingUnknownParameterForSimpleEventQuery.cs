using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.Queries
{
    [TestClass]
    public class WhenValidatingUnknownParameterForSimpleEventQuery : SimpleEventQueryParameterValidationFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Parameters = new QueryParameter[]
            {
                new QueryParameter{ Name = "unknown_parameter", Values = new []{ "test" } }
            };
        }

        [Assert]
        public void ItShouldThrowAnException() => Assert.IsNotNull(Catched);

        [Assert]
        public void TheExceptionShouldBeAnEpcisException() => Assert.IsInstanceOfType(Catched, typeof(EpcisException));
    }
}
