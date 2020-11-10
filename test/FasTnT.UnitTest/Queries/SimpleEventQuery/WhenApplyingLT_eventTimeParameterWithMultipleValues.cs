using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingLT_eventTimeParameterWithMultipleValues : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "LT_eventTime", Values = new[] { "2018-04-01T10:21:10.521Z", "2018-04-05T10:21:10.521Z" } });
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod]
        public void TheMessageShouldBeMeaningful()
        {
            Assert.AreEqual("A single value is expected, but multiple were found. Parameter name 'LT_eventTime'", Catched.Message);
        }
    }
}
