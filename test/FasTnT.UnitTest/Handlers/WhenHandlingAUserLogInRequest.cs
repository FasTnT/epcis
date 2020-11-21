using FasTnT.Domain.Commands.Requests;
using FasTnT.Domain.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Model.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Handlers
{
    [TestClass]
    public class WhenHandlingAUserLogInRequest : TestBase
    {
        public Mock<IUserManager> UserManager { get; set; }
        public UserLogInHandler Handler { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public UserLogInRequest Request { get; set; }
        public UserLogInResponse Response { get; set; }

        public override void Given()
        {
            UserManager = new Mock<IUserManager>();
            Handler = new UserLogInHandler(UserManager.Object);
            CancellationToken = new CancellationTokenSource().Token;
            Request = new UserLogInRequest
            {
                Password = "test123",
                Username = "TestUser"
            };

            UserManager.Setup(x => x.GetByUsername("TestUser", It.IsAny<CancellationToken>())).Returns(Task.FromResult(new User { UserName = "TestUser", Password = "e9c53617cce5d7467127d206473367f18e95e8f42b71eaadff0e614799484298" }));
        }

        public override void When()
        {
            Response = Handler.Handle(Request, CancellationToken).Result;
        }

        [TestMethod]
        public void ItShouldReturnAUserLogInResponse()
        {
            Assert.IsNotNull(Response);
        }

        [TestMethod]
        public void TheUserShouldBeAuthorized()
        {
            Assert.AreEqual(true, Response.Authorized);
        }

        [TestMethod]
        public void TheResponseShouldContainTheUserRetrievedFromTheDatabase()
        {
            Assert.IsNotNull(Response.User);
        }
    }
}
