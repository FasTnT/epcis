using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.Queries
{
    [TestClass]
    public class WhenValidatingNullParametersForSimpleEventQuery : SimpleEventQueryParameterValidationFixture
    {
        [Assert]
        public void ItShouldNotThrowAnException() => Assert.IsNull(Catched);
    }
}
