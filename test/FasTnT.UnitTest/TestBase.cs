using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest
{
    public abstract class TestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            Given();
            When();
        }

        public abstract void When();
        public abstract void Given();
    }
}
