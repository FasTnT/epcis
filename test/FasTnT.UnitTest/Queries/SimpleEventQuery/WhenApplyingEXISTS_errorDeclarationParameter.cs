using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEXISTS_errorDeclarationParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "EXISTS_errorDeclaration", Values = null });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithExistsErrorDeclarationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.IsAny<ExistsErrorDeclarationFilter>()), Times.Once);
        }
    }
}
