using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FasTnT.UnitTest.XmlFormatter
{
    [TestClass]
    public class WhenParsingAValidEventRequest : BaseUnitTest
    {
        public IRequestFormatter RequestParser { get; set; }
        public Stream InputFile { get; set; }
        public Request ParsedFile { get; set; }
        public EpcisEventDocument ParsedEvents => ParsedFile as EpcisEventDocument;

        public override void Arrange()
        {
            RequestParser = new XmlRequestFormatter();
            InputFile = File.OpenRead("XmlFormatter/files/requests/xml/valid_event_1.xml");
        }

        public override void Act()
        {
            ParsedFile = RequestParser.Read(InputFile) as EpcisEventDocument;
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

        [Assert]
        public void TheEventShouldBeAnObjectEvent()
        {
            var parsedEvent = ParsedEvents.EventList.First();
            Assert.AreEqual(EventType.Object, parsedEvent.Type);
        }

        [Assert]
        public void TheEventShouldHaveAddAction()
        {
            var parsedEvent = ParsedEvents.EventList.First();
            Assert.AreEqual(EventAction.Add, parsedEvent.Action);
        }

        [Assert]
        public void TheEventEPCListShouldContainThreeValues()
        {
            var parsedEvent = ParsedEvents.EventList.First();
            Assert.AreEqual(3, parsedEvent.Epcs.Count);
        }

        [Assert]
        public void TheEventEPCListShouldAllBeListType()
        {
            var parsedEvent = ParsedEvents.EventList.First();
            Assert.IsTrue(parsedEvent.Epcs.All(x => x.Type == EpcType.List));
        }

        [Assert]
        public void TheEventBusinessStepShouldBePacking()
        {
            var parsedEvent = ParsedEvents.EventList.First();
            Assert.AreEqual("urn:epcglobal:bizstep:packing", parsedEvent.BusinessStep);
        }

        [Assert]
        public void TheEventDispositionShouldBeLoading()
        {
            var parsedEvent = ParsedEvents.EventList.First();
            Assert.AreEqual("urn:epcglobal:cbv:disp:loading", parsedEvent.Disposition);
        }

        [Assert]
        public void TheEventReadPointShouldBeCorrectlyParsed()
        {
            var parsedEvent = ParsedEvents.EventList.First();
            Assert.AreEqual("urn:fastnt:demo:readpoint:0037000.00729.210,432", parsedEvent.ReadPoint);
        }

        [Assert]
        public void TheEventBusinessLocationShouldBeCorrectlyParsed()
        {
            var parsedEvent = ParsedEvents.EventList.First();
            Assert.AreEqual("urn:fastnt:demo:bizloc:0037000.00729.210", parsedEvent.BusinessLocation);
        }
    }
}
