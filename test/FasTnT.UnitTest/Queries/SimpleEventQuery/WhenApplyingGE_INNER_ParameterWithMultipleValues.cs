using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingGE_INNER_ParameterWithMultipleValues : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "GE_INNER_namespace#name", Values = new[] { "15", "78" } });
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod]
        public void TheMessageShouldBeMeaningful()
        {
            Assert.AreEqual("A single value is expected, but multiple were found. Parameter name 'GE_INNER_namespace#name'", Catched.Message);
        }
    }
}
