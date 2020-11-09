using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEXISTS_ILMDParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EXISTS_ILMD_namespace#ilmdName", Values = new string[0] });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSourceDestinationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<ExistCustomFieldFilter>(f => f.IsInner == false && f.Field.Namespace == "namespace" && f.Field.Name == "ilmdName" && f.Field.Type == FieldType.Ilmd)), Times.Once);
        }
    }
}
