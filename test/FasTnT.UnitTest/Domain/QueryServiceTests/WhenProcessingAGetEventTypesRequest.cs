using FasTnT.Domain;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Responses;
using FasTnT.Model.Utils;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FasTnT.UnitTest.Domain.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAGetEventTypesRequest : BaseQueryServiceUnitTest
    {
        static readonly string ExpectedVersion = Constants.ProductVersion;
        static readonly string[] ExpectedResult = Enumeration.GetAll<EventType>().Select(x => x.DisplayName).ToArray();

        public GetEventTypesResponse Response { get; set; }

        public override void Act() => Response = QueryService.GetEventTypes(default).Result;

        [Assert]
        public void TheResponseShouldNotBeNull() => Assert.IsNotNull(Response);

        [Assert]
        public void TheResponseShouldContainAllEventTypes() => CollectionAssert.AreEquivalent(ExpectedResult, Response.EventTypes.ToArray());
    }
}
