using FakeItEasy;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Subscriptions;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.SubscriptionServiceTests
{
    [TestClass]
    public class WhenProcessingASubscriptionRequestGivenSubscriptionAlreadyExist : BaseSubscriptionServiceUnitTest
    {
        public ISubscriptionManager SubscriptionManager { get; set; }
        public Subscription Request { get; set; }
        public Exception Catched { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            SubscriptionManager = A.Fake<ISubscriptionManager>();
            Request = new Subscription { SubscriptionId = "TestSubscription", Destination = "http://test.com/callback", QueryName = EpcisQueries.First(x => x.AllowSubscription).Name };

            A.CallTo(() => UnitOfWork.SubscriptionManager).Returns(SubscriptionManager);
            A.CallTo(() => SubscriptionManager.GetById("TestSubscription")).Returns(Task.FromResult(new Subscription()));
        }

        public override void Act()
        {
            try
            {
                Task.WaitAll(SubscriptionService.Process(Request));
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
        public void TheExceptionTypeShouldBeNoSubscribeNotPermittedException() => Assert.AreEqual(ExceptionType.SubscribeNotPermittedException, ((EpcisException)Catched).ExceptionType);

        [Assert]
        public void ItShouldHaveBeginTheUnitOfWorkTransaction() => A.CallTo(() => UnitOfWork.BeginTransaction()).MustHaveHappened();

        [Assert]
        public void ItShouldCallTheSubscriptionManagerProperty() => A.CallTo(() => UnitOfWork.SubscriptionManager).MustHaveHappened();

        [Assert]
        public void ItShouldNotAddTheSubscriptionToTheService() => A.CallTo(() => SubscriptionBackgroundService.Register(A<Subscription>._)).MustNotHaveHappened();
    }
}
