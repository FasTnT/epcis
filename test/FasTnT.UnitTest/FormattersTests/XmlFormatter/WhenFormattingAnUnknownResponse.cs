using FasTnT.Formatters.Xml;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace FasTnT.UnitTest.XmlFormatter
{
    [TestClass]
    public class WhenFormattingAnUnknownResponse : BaseUnitTest
    {
        public XmlResponseFormatter Formatter { get; set; }
        public IEpcisResponse Response { get; set; }
        public XDocument Result { get; set; }
        public Exception Catched { get; set; }

        public override void Arrange()
        {
            Formatter = new XmlResponseFormatter();
            Response = default;
        }

        public override void Act()
        {
            try
            {
                Result = Formatter.Format(Response);
            }
            catch(Exception ex)
            {
                Catched = ex;
            }
        }

        [Assert]
        public void TheXMLDocumentShouldBeNull()
        {
            Assert.IsNull(Result);
        }

        [Assert]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }
    }
}
