using FakeItEasy;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services;
using FasTnT.UnitTest.Common;

namespace FasTnT.UnitTest.Domain.CaptureServiceTests
{
    public abstract class BaseCaptureServiceUnitTest : BaseUnitTest
    {
        public IUnitOfWork UnitOfWork { get; set; } = A.Fake<IUnitOfWork>();
        public CaptureService CaptureService { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            CaptureService = new CaptureService(UnitOfWork);
        }
    }
}
