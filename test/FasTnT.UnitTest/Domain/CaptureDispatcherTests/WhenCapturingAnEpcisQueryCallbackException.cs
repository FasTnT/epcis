using FasTnT.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.CaptureDispatcherTests
{
    [TestClass]
    public class WhenCapturingAnEpcisQueryCallbackException : CaptureDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Document = new EpcisQueryCallbackException { };
        }

        [TestMethod]
        public void ItShouldNotThrowAnException()
        {
            Assert.IsNull(Exception);
        }
    }
}
