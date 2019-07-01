using FakeItEasy;
using FasTnT.Domain.Services;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.CaptureServiceTests
{
    [TestClass]
    public class WhenCapturingAQueryCallbackException : BaseCaptureServiceUnitTest
    {
        public EpcisQueryCallbackException Request { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new EpcisQueryCallbackException { SubscriptionName = "TestSubscription", CallbackType = QueryCallbackType.QueryTooLargeException, Reason = "Too much data returned by the query", Severity = "ERROR" };
        }

        public override void Act() => Task.WaitAll(CaptureService.CaptureCallbackException(Request, default));

        [Assert]
        public void ItShouldHaveBeginTheUnitOfWorkTransaction() => A.CallTo(() => UnitOfWork.BeginTransaction()).MustHaveHappened();

        [Assert]
        public void ItShouldHaveCommitTheTransaction() => A.CallTo(() => UnitOfWork.Commit()).MustHaveHappened();

        [Assert]
        public void ItShouldHaveAccessedTheCallbackStore() => A.CallTo(() => UnitOfWork.CallbackStore).MustHaveHappened();
    }
}
