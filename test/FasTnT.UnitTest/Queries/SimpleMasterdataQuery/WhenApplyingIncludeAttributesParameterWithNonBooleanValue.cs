using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    [TestClass]
    public class WhenApplyingIncludeAttributesParameterWithNonBooleanValue : SimpleMasterdataQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "includeAttributes", Values = new[] { "test" } });
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }
    }
}
