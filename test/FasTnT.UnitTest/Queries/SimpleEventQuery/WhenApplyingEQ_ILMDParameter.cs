using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEQ_ILMDParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EQ_ILMD_namespace#ilmd", Values = new[] { "expected" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSourceDestinationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<CustomFieldFilter>(f => f.IsInner == false && f.Field.Namespace == "namespace" && f.Field.Name == "ilmd" && f.Field.Type == FieldType.Ilmd && f.Values.Contains("expected"))), Times.Once);
        }
    }
}
