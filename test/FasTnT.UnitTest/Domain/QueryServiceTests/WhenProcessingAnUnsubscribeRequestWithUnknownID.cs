using FakeItEasy;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Subscriptions;
using FasTnT.UnitTest.Common;
using FasTnT.UnitTest.Domain.QueryServiceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.SubscriptionServiceTests
{
    [TestClass]
    public class WhenProcessingAnUnsubscribeRequestWithUnknownID : BaseQueryServiceUnitTest
    {
        public ISubscriptionManager SubscriptionManager { get; set; }
        public UnsubscribeRequest Request { get; set; }
        public Exception Catched { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            SubscriptionManager = A.Fake<ISubscriptionManager>();
            Request = new UnsubscribeRequest { SubscriptionId = "TestSubscription" };

            A.CallTo(() => UnitOfWork.SubscriptionManager).Returns(SubscriptionManager);
            A.CallTo(() => SubscriptionManager.GetById("TestSubscription", default)).Returns(Task.FromResult(default(Subscription)));
        }

        public override void Act()
        {
            try
            {
                Task.WaitAll(QueryService.Unsubscribe(Request, default));
            }
            catch (Exception ex)
            {
                Catched = (ex is AggregateException) ? ex.InnerException : ex;
            }
        }

        [Assert]
        public void ItShouldThrowAnException() => Assert.IsNotNull(Catched);

        [Assert]
        public void TheExceptionShouldBeEpcisException() => Assert.IsInstanceOfType(Catched, typeof(EpcisException));

        [Assert]
        public void TheExceptionTypeShouldBeNoSuchNameException() => Assert.AreEqual(ExceptionType.NoSuchSubscriptionException, ((EpcisException)Catched).ExceptionType);

        [Assert]
        public void ItShouldCallTheSubscriptionManagerProperty() => A.CallTo(() => UnitOfWork.SubscriptionManager).MustHaveHappened();

        [Assert]
        public void ItShouldNotRemoveTheSubscriptionFromTheService() => A.CallTo(() => SubscriptionBackgroundService.Remove(A<Subscription>._)).MustNotHaveHappened();
    }
}
