using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.Queries
{
    [TestClass]
    public class WhenValidatingGT_ParameterWithMultipleValuesForSimpleEventQuery : SimpleEventQueryParameterValidationFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Parameters = new QueryParameter[]
            {
                new QueryParameter{ Name = "GT_eventTime", Values = new []{ "2019-05-01T10:02:54Z", "2019-05-01T11:02:54Z" } }
            };
        }

        [Assert]
        public void ItShouldThrowAnException() => Assert.IsNotNull(Catched);

        [Assert]
        public void TheExceptionShouldBeEpcisException() => Assert.IsInstanceOfType(Catched, typeof(EpcisException));
    }
}
