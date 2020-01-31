using FasTnT.Domain.Commands.Setup;
using FasTnT.Domain.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using static FasTnT.Domain.Commands.Setup.MigrateDatabaseRequest;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAMigrateDatabaseRequest : TestBase
    {
        public Mock<IDatabaseMigrator> DatabaseMigrator { get; set; }
        public MigrateDatabaseHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public MigrateDatabaseRequest Request { get; set; }

        public override void Given()
        {
            DatabaseMigrator = new Mock<IDatabaseMigrator>();
            CancellationToken = new CancellationTokenSource().Token;
            Request = new MigrateDatabaseRequest();
            Handler = new MigrateDatabaseHandler(DatabaseMigrator.Object);
        }

        public override void When()
        {
            Task.WaitAll(Handler.Handle(Request, CancellationToken));
        }

        [TestMethod]
        public void ItShouldCallTheDatabaseMigratorMigrateMethod()
        {
            DatabaseMigrator.Verify(x => x.Migrate(), Times.Once);
        }
    }
}
