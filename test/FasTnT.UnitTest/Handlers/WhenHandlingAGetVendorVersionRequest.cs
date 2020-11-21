using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAGetVendorVersionRequest : TestBase
    {
        public GetVendorVersionHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public GetVendorVersionRequest Request { get; set; }
        public IEpcisResponse Response { get; set; }

        public override void Given()
        {
            CancellationToken = new CancellationTokenSource().Token;
            Request = new GetVendorVersionRequest();
            Handler = new GetVendorVersionHandler();
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldReturnAGetStandardVersionResponse()
        {
            Assert.IsInstanceOfType(Response, typeof(GetVendorVersionResponse));
        }

        [TestMethod]
        public void ItShouldReturnTheCurrentStandardVersion()
        {
            Assert.AreEqual(Constants.VendorVersion, ((GetVendorVersionResponse)Response).Version);
        }
    }
}
