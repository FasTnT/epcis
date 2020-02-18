using FasTnT.Domain.Data;
using FasTnT.Model.Queries;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    public abstract class SimpleEventQueryTestBase : TestBase
    {
        public Domain.Queries.SimpleEventQuery Query { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Mock<IEventFetcher> EventFetcher { get; set; }
        public IList<QueryParameter> Parameters { get; set; }
        public Exception Catched { get; set; }

        public override void Given()
        {
            EventFetcher = new Mock<IEventFetcher>();
            CancellationToken = new CancellationTokenSource().Token;
            Parameters = new List<QueryParameter>();
            Query = new Domain.Queries.SimpleEventQuery(EventFetcher.Object);
        }

        public override void When()
        {
            try
            {
                Task.WaitAll(Query.Handle(Parameters.ToArray(), CancellationToken));
            }
            catch(Exception ex)
            {
                Catched = ex?.InnerException ?? ex;
            }
        }
    }
}
