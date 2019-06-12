using FasTnT.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.CaptureDispatcherTests
{
    [TestClass]
    public class WhenCapturingAnEpcisQueryCallbackDocument : CaptureDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Document = new EpcisQueryCallbackDocument { };
        }

        [TestMethod]
        public void ItShouldNotThrowAnException()
        {
            Assert.IsNull(Exception);
        }
    }
}
