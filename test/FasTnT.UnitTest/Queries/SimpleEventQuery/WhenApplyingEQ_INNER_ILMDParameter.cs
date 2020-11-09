using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_INNER_ILMDParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_INNER_ILMD_namespace#ilmdInner", Values = new[] { "expectedValue" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSourceDestinationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<CustomFieldFilter>(f => f.IsInner == true && f.Field.Namespace == "namespace" && f.Field.Name == "ilmdInner" && f.Field.Type == FieldType.Ilmd && f.Values.Contains("expectedValue"))), Times.Once);
        }
    }
}
