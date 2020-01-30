namespace FasTnT.IntegrationTests.Common
{
    public abstract class BaseMigratedIntegrationTest : BaseIntegrationTest
    {
        public override void Arrange()
        {
            base.Arrange();
            RollbackDatabase();
            MigrateDatabase();
        }
    }
}
