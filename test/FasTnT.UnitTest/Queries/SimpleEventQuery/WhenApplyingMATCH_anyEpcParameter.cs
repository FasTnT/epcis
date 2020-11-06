using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingMATCH_anyEpcParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_anyEPC", Values = new[] { "any_epc" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMatchEpcFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MatchEpcFilter>(f => f.EpcType.Contains(EpcType.List) && f.EpcType.Contains(EpcType.ChildEpc) && f.EpcType.Contains(EpcType.ParentId) && f.EpcType.Contains(EpcType.InputEpc) && f.EpcType.Contains(EpcType.OutputEpc) && f.Values.Any(v => v.ToString() == "any_epc"))), Times.Once);
        }
    }
    [TestClass]
    public class WhenApplyingMATCH_epcParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_epc", Values = new[] { "epc_value" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMatchEpcFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MatchEpcFilter>(f => f.EpcType.Contains(EpcType.List) && f.EpcType.Contains(EpcType.ChildEpc) && f.Values.Any(v => v.ToString() == "epc_value"))), Times.Once);
        }
    }
    [TestClass]
    public class WhenApplyingMATCH_parentIDParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_parentID", Values = new[] { "epc_parent" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMatchEpcFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MatchEpcFilter>(f => f.EpcType.Contains(EpcType.ParentId) && f.Values.Any(v => v.ToString() == "epc_parent"))), Times.Once);
        }
    }
    [TestClass]
    public class WhenApplyingMATCH_inputEPCParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_inputEPC", Values = new[] { "inputEpc" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMatchEpcFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MatchEpcFilter>(f => f.EpcType.Contains(EpcType.InputEpc) && f.Values.Any(v => v.ToString() == "inputEpc"))), Times.Once);
        }
    }
    [TestClass]
    public class WhenApplyingMATCH_outputEPCParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_outputEPC", Values = new[] { "epc_value" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMatchEpcFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MatchEpcFilter>(f => f.EpcType.Contains(EpcType.OutputEpc) && f.Values.Any(v => v.ToString() == "epc_value"))), Times.Once);
        }
    }
    [TestClass]
    public class WhenApplyingMATCH_epcClassParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_epcClass", Values = new[] { "epc_value" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMatchEpcFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MatchEpcFilter>(f => f.EpcType.Contains(EpcType.Quantity) && f.EpcType.Contains(EpcType.ChildQuantity) && f.Values.Any(v => v.ToString() == "epc_value"))), Times.Once);
        }
    }
    [TestClass]
    public class WhenApplyingMATCH_inputEpcClassParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_inputEpcClass", Values = new[] { "epc_value" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMatchEpcFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MatchEpcFilter>(f => f.EpcType.Contains(EpcType.InputQuantity) && f.Values.Any(v => v.ToString() == "epc_value"))), Times.Once);
        }
    }
    [TestClass]
    public class WhenApplyingMATCH_outputEpcClassParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_outputEpcClass", Values = new[] { "epc_value" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMatchEpcFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MatchEpcFilter>(f => f.EpcType.Contains(EpcType.OutputQuantity) && f.Values.Any(v => v.ToString() == "epc_value"))), Times.Once);
        }
    }
    [TestClass]
    public class WhenApplyingMATCH_anyEpcClassParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_anyEpcClass", Values = new[] { "epc_value" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithMatchEpcFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<MatchEpcFilter>(f => f.EpcType.Contains(EpcType.OutputQuantity) && f.EpcType.Contains(EpcType.InputQuantity) && f.EpcType.Contains(EpcType.Quantity) && f.Values.Any(v => v.ToString() == "epc_value"))), Times.Once);
        }
    }
    [TestClass]
    public class WhenApplyingMATCH_unknownParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "MATCH_unknown", Values = new[] { "epc_value" } });
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod]
        public void TheExceptionMessageShouldSpecifyUnknownMatchParameter()
        {
            Assert.AreEqual("Unknown 'MATCH_*' parameter: 'MATCH_unknown'", Catched.Message);
        }
    }
}
