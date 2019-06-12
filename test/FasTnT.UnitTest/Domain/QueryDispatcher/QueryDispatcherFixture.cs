using FakeItEasy;
using FasTnT.Domain;
using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services;
using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using System;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    public abstract class QueryDispatcherFixture : BaseUnitTest
    {
        public QueryDispatcher QueryDispatcher { get; set; }
        public QueryService QueryService { get; set; }
        public EpcisQuery Query { get; set; }
        public IEpcisResponse Response { get; set; }
        public Exception Exception { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            QueryService = A.Fake<QueryService>(o => o.WithArgumentsForConstructor(() => new QueryService(new[] { new SimpleEventQuery() }, A.Fake<IUnitOfWork>(), A.Fake<ISubscriptionBackgroundService>())));
            QueryDispatcher = new QueryDispatcher(QueryService);
        }

        public override void Act()
        {
            try
            {
                Response = QueryDispatcher.DispatchQuery(Query, default).Result;
            }
            catch(Exception ex)
            {
                Exception = ex;
            }
        }
    }
}
