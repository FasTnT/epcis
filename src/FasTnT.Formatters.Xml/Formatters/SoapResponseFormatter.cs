using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters
{
    public class SoapResponseFormatter : BaseXmlFormatter
    {
        internal override XDocument WrapResponse(XElement response)
        {
            return new XDocument(new XElement(XName.Get("Envelope", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XAttribute(XNamespace.Xmlns + "epcisq", "urn:epcglobal:epcis-query:xsd:1"),
                new XElement(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/"), response)
            ));
        }
    }
}
