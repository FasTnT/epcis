using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace FasTnT.IntegrationTests.Common
{
    public abstract class BaseIntegrationTest
    {
        public HttpClient Client { get; set; }
        public HttpResponseMessage Result { get; set; }

        public virtual void Arrange()
        {
            Client = IntegrationTest.Client;
        }

        public abstract void Act();

        [TestInitialize]
        public void Execute()
        {
            Arrange();
            Act();
        }
    }
}
