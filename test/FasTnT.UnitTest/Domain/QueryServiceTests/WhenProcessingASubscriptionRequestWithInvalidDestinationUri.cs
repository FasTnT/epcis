﻿using FakeItEasy;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Subscriptions;
using FasTnT.UnitTest.Common;
using FasTnT.UnitTest.Domain.QueryServiceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.SubscriptionServiceTests
{
    [TestClass]
    public class WhenProcessingASubscriptionRequestWithInvalidDestinationUri : BaseQueryServiceUnitTest
    {
        public ISubscriptionManager SubscriptionManager { get; set; }
        public Subscription Request { get; set; }
        public Exception Catched { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            SubscriptionManager = A.Fake<ISubscriptionManager>();
            Request = new Subscription { SubscriptionId = "TestSubscription", Trigger = "trigger", Destination = "NotAValidUri", QueryName = EpcisQueries.First(x => x.AllowSubscription).Name };

            A.CallTo(() => UnitOfWork.SubscriptionManager).Returns(SubscriptionManager);
            A.CallTo(() => SubscriptionManager.GetById("TestSubscription", default)).Returns(Task.FromResult(default(Subscription)));
        }

        public override void Act()
        {
            try
            {
                Task.WaitAll(QueryService.Subscribe(Request, default));
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
        public void TheExceptionTypeShouldBeNoSubscribeNotPermittedException() => Assert.AreEqual(ExceptionType.InvalidURIException, ((EpcisException)Catched).ExceptionType);

        [Assert]
        public void ItShouldNotHaveBeginTheUnitOfWorkTransaction() => A.CallTo(() => UnitOfWork.BeginTransaction()).MustNotHaveHappened();

        [Assert]
        public void ItShouldNotHaveCommitTheTransaction() => A.CallTo(() => UnitOfWork.Commit()).MustNotHaveHappened();

        [Assert]
        public void ItShouldNotCallTheSubscriptionManagerProperty() => A.CallTo(() => UnitOfWork.SubscriptionManager).MustNotHaveHappened();

        [Assert]
        public void ItShouldNotAddTheSubscriptionToTheService() => A.CallTo(() => SubscriptionBackgroundService.Register(A<Subscription>._)).MustNotHaveHappened();
    }
}