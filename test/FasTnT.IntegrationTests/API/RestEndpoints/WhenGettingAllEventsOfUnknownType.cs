using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace FasTnT.IntegrationTests.API.RestEndpoints
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class WhenGettingAllEventsOfUnknownType : BaseMigratedIntegrationTest
    {
        public override void Act()
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");
            var request = new HttpRequestMessage(HttpMethod.Get, "/v2_0/events/UnknownEvent/all") { Content = new StringContent("", Encoding.UTF8, "application/json") };
            Result = Client.SendAsync(request).Result;
        }

        [Assert]
        public void ItShouldReturnHttp400BadRequest() => Assert.AreEqual(HttpStatusCode.BadRequest, Result.StatusCode);

        [Assert]
        public void ItShouldReturnAValidJson() => Assert.IsNotNull(JsonConvert.DeserializeObject(Result.Content.ReadAsStringAsync().Result));

        [Assert]
        public void ItShouldReturnAnExceptionWith() => Assert.AreEqual("QueryParameterException", (JsonConvert.DeserializeObject<IDictionary<string, object>>(Result.Content.ReadAsStringAsync().Result))["exception"]);
    }
}
