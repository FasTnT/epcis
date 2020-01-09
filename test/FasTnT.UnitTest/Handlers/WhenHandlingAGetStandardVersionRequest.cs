using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain;
using FasTnT.Domain.Handlers.GetStandardVersion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAGetStandardVersionRequest : TestBase
    {
        public GetStandardVersionHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public GetStandardVersionRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            CancellationToken = new CancellationTokenSource().Token;
            Request = new GetStandardVersionRequest();
            Handler = new GetStandardVersionHandler();
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldReturnAGetStandardVersionResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(GetStandardVersionResponse));
        }

        [TestMethod]
        public void ItShouldReturnTheCurrentStandardVersion()
        {
            Assert.AreEqual(Constants.StandardVersion, ((GetStandardVersionResponse)Response).Version);
        }
    }
}
