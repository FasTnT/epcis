using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingLT_INNER_ILMDParameterWithNumericValue : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "LT_INNER_ILMD_namespace#ilmd", Values = new[] { "15" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSourceDestinationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<ComparisonCustomFieldFilter>(f => f.IsInner == true && f.Comparator == FilterComparator.LessThan && f.Field.Namespace == "namespace" && f.Field.Name == "ilmd" && f.Field.Type == FieldType.Ilmd && (double)f.Value == 15)), Times.Once);
        }
    }
}
