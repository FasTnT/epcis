using FakeItEasy;
using FasTnT.Domain.Services;
using FasTnT.Model;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.CaptureServiceTests
{
    [TestClass]
    public class WhenCapturingAnEpcisEventDocument : BaseCaptureServiceUnitTest
    {
        public EpcisEventDocument Request { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new EpcisEventDocument { Header = new EpcisRequestHeader(), EventList = new EpcisEvent[0] };
        }

        public override void Act() => Task.WaitAll(CaptureService.Capture(Request));

        [Assert]
        public void ItShouldHaveBeginTheUnitOfWorkTransaction() => A.CallTo(() => UnitOfWork.BeginTransaction()).MustHaveHappened();

        [Assert]
        public void ItShouldHaveCommitTheTransaction() => A.CallTo(() => UnitOfWork.Commit()).MustHaveHappened();

        [Assert]
        public void ItShouldHaveAccessedTheRequestStore() => A.CallTo(() => UnitOfWork.RequestStore).MustHaveHappened();

        [Assert]
        public void ItShouldHaveAccessedTheEventStore() => A.CallTo(() => UnitOfWork.EventStore).MustHaveHappened();
    }
}
