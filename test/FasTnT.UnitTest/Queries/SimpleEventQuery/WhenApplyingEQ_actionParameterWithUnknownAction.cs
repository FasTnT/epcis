using FasTnT.Model.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_actionParameterWithUnknownAction : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_action", Values = new[] { "UNKNOWN" } });
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod, Ignore("Yet to be implemented")]
        public void ItShouldThrowAnEpcisException()
        {
            Assert.IsInstanceOfType(Catched, typeof(EpcisException));
        }
    }
}
