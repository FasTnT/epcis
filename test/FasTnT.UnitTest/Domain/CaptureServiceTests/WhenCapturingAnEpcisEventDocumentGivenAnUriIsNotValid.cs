using FakeItEasy;
using FasTnT.Domain.Services;
using FasTnT.Model;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.CaptureServiceTests
{
    [TestClass]
    public class WhenCapturingAnEpcisEventDocumentGivenAnUriIsNotValid : BaseCaptureServiceUnitTest
    {
        public CaptureRequest Request { get; set; }
        public Exception Catched { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new CaptureRequest { Header = new EpcisRequestHeader(), EventList = new EpcisEvent[]
                {
                    new EpcisEvent
                    {
                        Epcs = new Epc[]{ new Epc { Id = "NotAValidUri" } }
                    }
                }
            };
        }

        public override void Act()
        {
            try
            {
                Task.WaitAll(CaptureService.Capture(Request, default));
            }
            catch(AggregateException ex)
            {
                Catched = ex.InnerException;
            }
            catch(Exception ex)
            {
                Catched = ex;
            }
        }

        [Assert]
        public void ItShouldThrowAnException() => Assert.IsNotNull(Catched);

        [Assert]
        public void ItShouldHaveBeginTheUnitOfWorkTransaction() => A.CallTo(() => UnitOfWork.BeginTransaction()).MustNotHaveHappened();

        [Assert]
        public void ItShouldHaveCommitTheTransaction() => A.CallTo(() => UnitOfWork.Commit()).MustNotHaveHappened();

        [Assert]
        public void ItShouldHaveAccessedTheRequestStore() => A.CallTo(() => UnitOfWork.RequestStore).MustNotHaveHappened();

        [Assert]
        public void ItShouldHaveAccessedTheEventStore() => A.CallTo(() => UnitOfWork.EventStore).MustNotHaveHappened();
    }
}
