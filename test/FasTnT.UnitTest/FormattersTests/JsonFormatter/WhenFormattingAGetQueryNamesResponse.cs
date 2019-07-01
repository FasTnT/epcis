using FasTnT.Formatters.Json;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace FasTnT.UnitTest.FormattersTests.JsonFormatter
{
    [TestClass]
    public class WhenFormattingAGetQueryNamesResponse : BaseUnitTest
    {
        public IEpcisResponse Response { get; set; }
        public JsonResponseFormatter Formatter { get; set; }
        public string Formatted { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Formatter = new JsonResponseFormatter();
            Response = new GetQueryNamesResponse { QueryNames = new[] { "SimpleEventQuery", "SimpleMasterdataQuery" } };
        }

        public override void Act()
        {
            Formatted = Formatter.Format(Response);
        }

        [Assert]
        public void ItShouldReturnANotNullString()
        {
            Assert.IsNotNull(Formatted);
            Assert.IsFalse(string.IsNullOrWhiteSpace(Formatted), "The result should not be empty or white spaces");
        }

        [Assert]
        public void ItShouldReturnAJsonArray()
        {
            var array = JArray.Parse(Formatted);
            Assert.IsNotNull(array);
        }

        [Assert]
        public void ItShouldReturnAnArrayContainingAllTheQueryNames()
        {
            var array = JArray.Parse(Formatted);
            Assert.IsTrue(array.Any(x => x.Value<string>() == "SimpleEventQuery"));
            Assert.IsTrue(array.Any(x => x.Value<string>() == "SimpleMasterdataQuery"));
        }
    }
}
