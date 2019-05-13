using FasTnT.IntegrationTests.Common;

namespace FasTnT.IntegrationTests.Common
{
    public abstract class BaseRollbackedIntegrationTest : BaseIntegrationTest
    {
        public override void Arrange()
        {
            base.Arrange();
            RollbackDatabase();
        }
    }
}
