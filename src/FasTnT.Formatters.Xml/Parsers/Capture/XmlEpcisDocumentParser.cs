using FasTnT.Formatters.Xml.Parsers.Capture.Events;
using FasTnT.Model;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Capture
{
    internal static class XmlEpcisDocumentParser
    {
        internal static EpcisRequest Parse(XElement root)
        {
            var request = new EpcisRequest
            {
                DocumentTime = DateTime.Parse(root.Attribute("creationDate").Value),
                SchemaVersion = root.Attribute("schemaVersion").Value
            };

            ParseHeaderIntoRequest(root.Element("EPCISHeader"), request);
            ParseBodyIntoRequest(root.Element("EPCISBody"), request);

            return request;
        }

        private static void ParseHeaderIntoRequest(XElement epcisHeader, EpcisRequest request)
        {
            if (epcisHeader == null || epcisHeader.IsEmpty) return;

            var headerMasterdata = epcisHeader.Element("extension")?.Element("EPCISMasterData")?.Element("VocabularyList");

            if (headerMasterdata != null)
            {
                request.MasterdataList.AddRange(XmlMasterdataParser.ParseMasterdata(headerMasterdata));
            }
        }

        private static void ParseBodyIntoRequest(XElement epcisBody, EpcisRequest request)
        {
            var element = epcisBody.Elements().First();

            switch (element.Name.LocalName)
            {
                case "EventList":
                    request.EventList.AddRange(XmlEventParser.ParseEvents(element));
                    break;
                case "VocabularyList":
                    request.MasterdataList.AddRange(XmlMasterdataParser.ParseMasterdata(element));
                    break;
            }
        }
    }
}
