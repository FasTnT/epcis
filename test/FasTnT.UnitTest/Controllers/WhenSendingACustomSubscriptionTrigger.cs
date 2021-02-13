using FasTnT.Domain.Notifications;
using FasTnT.Host.Controllers.v1_2;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Controllers
{
    [TestClass]
    public class WhenSendingACustomSubscriptionTrigger : TestBase
    {
        public SubscriptionController Controller { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public string TriggerName { get; set; }

        public override void Given()
        {
            Mediator = new Mock<IMediator>();
            TriggerName = "test";
            CancellationToken = new CancellationTokenSource().Token;
            Controller = new SubscriptionController(Mediator.Object);
        }

        public override void When()
        {
            Task.WaitAll(Controller.TriggerSubscription(TriggerName, CancellationToken));
        }

        [TestMethod]
        public void ItShouldSendTheCommandToTheMediator()
        {
            Mediator.Verify(x => x.Publish(It.Is<TriggerSubscriptionNotification>(n => n.Name == "test"), CancellationToken), Times.Once);
        }
    }
}
