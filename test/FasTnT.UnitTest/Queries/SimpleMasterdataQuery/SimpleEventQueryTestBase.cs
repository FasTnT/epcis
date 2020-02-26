using FasTnT.Domain.Data;
using FasTnT.Model.Queries;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    public abstract class SimpleMasterdataQueryTestBase : TestBase
    {
        public Domain.Queries.SimpleMasterdataQuery Query { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Mock<IMasterdataFetcher> MasterdataFetcher { get; set; }
        public IList<QueryParameter> Parameters { get; set; }
        public Exception Catched { get; set; }

        public override void Given()
        {
            MasterdataFetcher = new Mock<IMasterdataFetcher>();
            CancellationToken = new CancellationTokenSource().Token;
            Parameters = new List<QueryParameter>();
            Query = new Domain.Queries.SimpleMasterdataQuery(MasterdataFetcher.Object);
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
