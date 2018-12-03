using FasTnT.Domain;
using FasTnT.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenParsingAValidEventRequest : BaseUnitTest
    {
        public IRequestFormatter RequestParser { get; set; }
        public Request ParsedFile { get; set; }
        public EpcisEventDocument ParsedEvents => ParsedFile as EpcisEventDocument;

        public override void Arrange()
        {
            RequestParser = new XmlRequestFormatter();
        }

        public override void Act()
        {
            var input = File.OpenRead("files/requests/xml/valid_event_1.xml");

            ParsedFile = RequestParser.Read(input) as EpcisEventDocument;
        }

        [Assert]
        public void ItShouldParseCorrectlyTheSchemaVersion()
        {
            Assert.AreEqual("1.2", ParsedFile.SchemaVersion);
        }

        [Assert]
        public void ItShouldParseCorrectlyTheDocumentDate()
        {
            var expectedDate = DateTime.Parse("2017-09-19T12:57:12Z", CultureInfo.InvariantCulture);

            Assert.AreEqual(expectedDate, ParsedFile.CreationDate);
        }

        [Assert]
        public void TheParsedDocumentShouldBeAnEpcisEventDocument()
        {
            Assert.IsInstanceOfType(ParsedFile, typeof(EpcisEventDocument));
        }

        [Assert]
        public void ItShouldReturnOneEvent()
        {
            Assert.AreEqual(1, ParsedEvents.EventList.Count());
        }
    }
}
