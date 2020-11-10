using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEXISTS_INNERParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EXISTS_INNER_namespaceInner#name", Values = new string[0] });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSourceDestinationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<ExistCustomFieldFilter>(f => f.IsInner == true && f.Field.Namespace == "namespaceInner" && f.Field.Name == "name" && f.Field.Type == FieldType.CustomField)), Times.Once);
        }
    }
}
