using FasTnT.Domain.Data;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Domain.Queries;
using FasTnT.Subscriptions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Subscriptions
{

    public abstract class SubscriptionRunnerTests : TestBase
    {
        public SubscriptionRunner Runner { get; set; }
        public IEnumerable<IEpcisQuery> Queries { get; set; }
        public Mock<ISubscriptionManager> SubscriptionManager { get; set; }
        public Mock<IEventFetcher> EventFetcher { get; set; }
        public Mock<ISubscriptionResultSender> ResultSender { get; set; }

        public Subscription Subscription { get; set; }

        public override void Given()
        {
            SubscriptionManager = new Mock<ISubscriptionManager>(MockBehavior.Loose);
            ResultSender = new Mock<ISubscriptionResultSender>(MockBehavior.Loose);
            EventFetcher = new Mock<IEventFetcher>();
            Queries = new[] { new SimpleEventQuery(EventFetcher.Object) };

            Runner = new SubscriptionRunner(Queries, SubscriptionManager.Object, ResultSender.Object);
        }

        public override void When()
        {
            Task.WaitAll(Runner.Run(Subscription, default));
        }
    }
}
