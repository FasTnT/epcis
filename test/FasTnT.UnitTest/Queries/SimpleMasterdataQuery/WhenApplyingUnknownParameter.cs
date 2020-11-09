using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    [TestClass]
    public class WhenApplyingUnknownParameter : SimpleMasterdataQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "unknown", Values = new[] { "10" } });
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
            Assert.IsInstanceOfType(Catched, typeof(NotImplementedException));
        }

        [TestMethod]
        public void TheExceptionShouldHaveAMeaningfulMessage()
        {
            Assert.AreEqual("Parameter 'unknown' is not implemented yet.", Catched.Message);
        }
    }
}
