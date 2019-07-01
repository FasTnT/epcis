using FasTnT.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.CaptureDispatcherTests
{
    [TestClass]
    public class WhenCapturingACaptureRequestDocument : CaptureDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Document = new CaptureRequest { };
        }

        [TestMethod]
        public void ItShouldNotThrowAnException()
        {
            Assert.IsNull(Exception);
        }
    }
}
