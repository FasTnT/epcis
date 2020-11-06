using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    [TestClass]
    public class WhenApplyingIncludeAttributesParameterWithMultipleValues : SimpleMasterdataQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "includeAttributes", Values = new[] { "true", "false" } });
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod]
        public void TheMessageShouldBeMeaningful()
        {
            Assert.AreEqual("A single value is expected, but multiple were found. Parameter name 'includeAttributes'", Catched.Message);
        }
    }
}
