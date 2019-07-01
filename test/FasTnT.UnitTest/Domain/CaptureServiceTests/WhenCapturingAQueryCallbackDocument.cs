using FakeItEasy;
using FasTnT.Domain.Services;
using FasTnT.Model;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.CaptureServiceTests
{
    [TestClass]
    public class WhenCapturingAQueryCallbackDocument : BaseCaptureServiceUnitTest
    {
        public EpcisQueryCallbackDocument Request { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new EpcisQueryCallbackDocument { EventList = new EpcisEvent[0], SubscriptionName = "TestSubscription" };
        }

        public override void Act() => Task.WaitAll(CaptureService.CaptureCallback(Request, default));

        [Assert]
        public void ItShouldHaveBeginTheUnitOfWorkTransaction() => A.CallTo(() => UnitOfWork.BeginTransaction()).MustHaveHappened();

        [Assert]
        public void ItShouldHaveCommitTheTransaction() => A.CallTo(() => UnitOfWork.Commit()).MustHaveHappened();

        [Assert]
        public void ItShouldHaveAccessedTheCallbackStore() => A.CallTo(() => UnitOfWork.CallbackStore).MustHaveHappened();
    }
}
