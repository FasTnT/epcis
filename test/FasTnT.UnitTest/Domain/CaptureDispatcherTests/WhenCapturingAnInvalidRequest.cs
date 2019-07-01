using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.CaptureDispatcherTests
{
    [TestClass]
    public class WhenCapturingAnInvalidRequest : CaptureDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Document = default;
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Exception);
        }
    }
}
