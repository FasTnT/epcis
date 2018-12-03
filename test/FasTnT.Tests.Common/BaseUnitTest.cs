using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Tests.Common
{
    /// <summary>
    /// Base class to implement the Arrange/Act/Assert tests
    /// </summary>
    public abstract class BaseUnitTest
    {
        /// <summary>
        /// Setup all dependencies and needed objects for the test
        /// </summary>
        public virtual void Arrange() { }

        /// <summary>
        /// Executes the line to be tested
        /// </summary>
        public abstract void Act();

        /// <summary>
        /// Arranges and Act the test
        /// </summary>
        [TestInitialize]
        public void Execute()
        {
            Arrange();
            Act();
        }
    }
}
