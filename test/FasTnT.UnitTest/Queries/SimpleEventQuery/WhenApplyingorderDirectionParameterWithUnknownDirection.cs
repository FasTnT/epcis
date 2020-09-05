using FasTnT.Model.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingorderDirectionParameterWithUnknownDirection : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "orderDirection", Values = new[] { "UNKNOWN" } });
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
            Assert.IsInstanceOfType(Catched, typeof(EpcisException));
        }
    }
}
