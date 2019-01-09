using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using FasTnT.Formatters.Xml.Requests;
using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;

namespace FasTnT.Formatters.Xml
{
    public class XmlRequestFormatter : IRequestFormatter
    {
        public Request Read(Stream input)
        {
            var document = XDocument.Load(input);

            if (document.Root.Name == XName.Get("EPCISDocument", EpcisNamespaces.Capture))
            {
                return new EpcisEventDocument
                {
                    CreationDate = DateTime.Parse(document.Root.Attribute("creationDate").Value, CultureInfo.InvariantCulture),
                    SchemaVersion = document.Root.Attribute("schemaVersion").Value,
                    EventList = XmlEventsParser.ParseEvents(document.Root.XPathSelectElement("EPCISBody/EventList").Elements().ToArray())
                };
            }
            // TODO: handle masterdata document.
            //else if (document.Root.Name == XName.Get("EPCISMasterDataDocument", EpcisNamespaces.Capture))
            //{
            //    return new EpcisMasterdataDocument();
            //}

            throw new Exception($"Document with root '{document.Root.Name.ToString()}' is not expected here.");
        }

        public void Write(Request entity, Stream output)
        {
            XDocument document = Write((dynamic)entity);
            var bytes = Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces));

            output.Write(bytes, 0, bytes.Length);
        }

        private XDocument Write(EpcisEventDocument entity)
        {
            return new XDocument(
                XName.Get("EPCISDocument", EpcisNamespaces.Capture),
                new XAttribute("creationDate", entity.CreationDate.ToString("yyyy-MM-ddThh:MM:ssZ")),
                new XAttribute("schemaVersion", entity.SchemaVersion),
                new XElement("EPCISBody", new XElement("EventList", entity.EventList.Select(XmlEventFormatter.Format)))
            );
        }
    }
}
