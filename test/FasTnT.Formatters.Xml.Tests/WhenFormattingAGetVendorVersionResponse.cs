using FasTnT.Model.Responses;
using FasTnT.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenFormattingAGetVendorVersionResponse : BaseUnitTest
    {
        public XmlResponseFormatter Formatter { get; set; }
        public GetVendorVersionResponse GetVendorVersionResponse { get; set; }
        public XDocument Result { get; set; }

        public override void Arrange()
        {
            Formatter = new XmlResponseFormatter();
            GetVendorVersionResponse = new GetVendorVersionResponse { Version = "0.5" };
        }

        public override void Act()
        {
            Result = Formatter.Format(GetVendorVersionResponse);
        }

        [Assert]
        public void TheXMLDocumentShouldNotBeNull()
        {
            Assert.IsNotNull(Result);
        }

        [Assert]
        public void TheXMLDocumentShouldContainAnEPCISQueryDocumentRoot()
        {
            Assert.AreEqual("EPCISQueryDocument", Result.Root.Name.LocalName);
        }

        [Assert]
        public void TheXMLDocumentRootShouldContainTheEPCISAttributes()
        {
            Assert.IsNotNull(Result.Root.Attributes().Where(x => x.Name == "creationDate").FirstOrDefault());
            Assert.IsNotNull(Result.Root.Attributes().Where(x => x.Name == "schemaVersion").FirstOrDefault());
        }
    }
}
