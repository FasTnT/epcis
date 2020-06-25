using FasTnT.Parsers.Xml.Utils;
using System;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters
{
    public class XmlResponseFormatter : BaseXmlFormatter
    {
        internal override XDocument WrapResponse(XElement response)
        {
            var rootElement = new XElement(XName.Get("EPCISQueryDocument", EpcisNamespaces.Query),
                new XAttribute(XNamespace.Xmlns + "epcisq", EpcisNamespaces.Query),
                new XAttribute("creationDate", DateTime.UtcNow), 
                new XAttribute("schemaVersion", "1.0"),
                new XElement("EPCISBody", response)
            );

            return new XDocument(rootElement);
        }
    }
}
