using FakeItEasy;
using FasTnT.Domain.Services;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.CaptureServiceTests
{
    [TestClass]
    public class WhenCapturingAnEpcisMasterDataDocument : BaseCaptureServiceUnitTest
    {
        public CaptureRequest Request { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new CaptureRequest { MasterDataList = new EpcisMasterData[0] };
        }

        public override void Act() => Task.WaitAll(CaptureService.CaptureDocument(Request, default));

        [Assert]
        public void ItShouldHaveBeginTheUnitOfWorkTransaction() => A.CallTo(() => UnitOfWork.BeginTransaction()).MustHaveHappened();

        [Assert]
        public void ItShouldHaveCommitTheTransaction() => A.CallTo(() => UnitOfWork.Commit()).MustHaveHappened();

        [Assert]
        public void ItShouldHaveAccessedTheMasterDataManager() => A.CallTo(() => UnitOfWork.MasterDataManager).MustHaveHappened();
    }
}
