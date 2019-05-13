using FakeItEasy;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Queries.Implementations;
using FasTnT.UnitTest.Common;

namespace FasTnT.UnitTest.Domain.Queries
{
    public abstract class QueryFixture : BaseUnitTest
    {
        public IEpcisQuery Query { get; set; }
        public IUnitOfWork UnitOfWork { get; set; } = A.Fake<IUnitOfWork>();
    }
}
