using FakeItEasy;
using FasTnT.Domain;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services;
using FasTnT.Domain.Services.Users;
using FasTnT.Model;
using FasTnT.UnitTest.Common;
using System;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.CaptureDispatcherTests
{
    public abstract class CaptureDispatcherFixture : BaseUnitTest
    {
        public CaptureDispatcher CaptureDispatcher { get; set; }
        public CaptureService CaptureService { get; set; }
        public Request Document { get; set; }
        public Exception Exception { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            var unitOfWork = A.Fake<IUnitOfWork>();

            CaptureService = A.Fake<CaptureService>(o => o.WithArgumentsForConstructor(() => new CaptureService(unitOfWork, A.Fake<UserContext>())));
            CaptureDispatcher = new CaptureDispatcher(CaptureService);
        }

        public override void Act()
        {
            try
            {
                Task.WaitAll(CaptureDispatcher.DispatchDocument(Document, default));
            }
            catch(Exception ex)
            {
                Exception = ex;
            }
        }
    }
}
