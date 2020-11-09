using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingUnknownParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "Unknown_param", Values = new[] { "someValue" } });
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod]
        public void TheExceptionShouldContainAMenaningfulMessage()
        {
            Assert.AreEqual("Query parameter unexpected or not implemented: 'Unknown_param'", Catched.Message);
        }
    }
}
