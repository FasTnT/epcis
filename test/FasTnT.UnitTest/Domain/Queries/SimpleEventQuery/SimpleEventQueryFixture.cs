using FasTnT.Model.Queries.Implementations;

namespace FasTnT.UnitTest.Domain.Queries
{
    public abstract class SimpleEventQueryFixture : QueryFixture
    {
        public override void Arrange()
        {
            base.Arrange();
            Query = new SimpleEventQuery();
        }
    }
}
