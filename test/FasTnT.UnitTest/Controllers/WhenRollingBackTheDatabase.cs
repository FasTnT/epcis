using FasTnT.Domain.Commands.Setup;
using FasTnT.Host.Controllers;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Controllers
{
    [TestClass]
    public class WhenRollingBackTheDatabase : TestBase
    {
        public EpcisMigrationController Controller { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            CancellationToken = new CancellationTokenSource().Token;
            Controller = new EpcisMigrationController(Mediator.Object);
        }

        public override void When()
        {
            Task.WaitAll(Controller.Rollback(CancellationToken));
        }

        [TestMethod]
        public void ItShouldSendTheCommandToTheMediator()
        {
            Mediator.Verify(x => x.Send(It.IsAny<RollbackDatabaseRequest>(), CancellationToken), Times.Once);
        }
    }
}
