using FakeItEasy;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Tests.Common;

namespace FasTnT.Domain.Tests.QueryServiceTests
{
    public abstract class BaseQueryServiceUnitTest : BaseUnitTest
    {
        public IUnitOfWork UnitOfWork { get; set; } = A.Fake<IUnitOfWork>();
        public IEpcisQuery[] EpcisQueries { get; set; } = new IEpcisQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };
        public QueryService QueryService { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            QueryService = new QueryService(EpcisQueries, UnitOfWork);
        }
    }
}
