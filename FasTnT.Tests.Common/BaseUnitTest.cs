using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Tests.Common
{
    public abstract class BaseUnitTest
    {
        public virtual void Arrange() { }
        public abstract void Act();

        [TestInitialize]
        public void Execute()
        {
            Arrange();
            Act();
        }
    }
}
