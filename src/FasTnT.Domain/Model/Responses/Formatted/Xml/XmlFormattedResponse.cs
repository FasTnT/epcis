using FasTnT.Domain.Services;
using System;
using System.IO;
using System.Xml.Linq;

namespace FasTnT.Domain.Model.Responses.Formatted
{
    public class XmlFormattedResponse : XDocument, IFormattedResponse
    {
        public static XAttribute Namespace = new XAttribute(XNamespace.Xmlns + "epcisq", EpcisNamespaces.Query);

        protected XmlFormattedResponse(string name, string nameSpace = "") : base(new XElement(XName.Get(name, nameSpace)))
        {
            Declaration = new XDeclaration("1.0", "UTF-8", null);
        }

        public static XmlFormattedResponse WithAttributes(string name, string nameSpace = "")
        {
            var response = new XmlFormattedResponse(name, nameSpace);
            response.Root.Add(new XAttribute("creationDate", DateTime.UtcNow), new XAttribute("schemaVersion", "1.2"), Namespace);

            return response;
        }

        public static XmlFormattedResponse WithoutAttributes(string name, string nameSpace = "") => new XmlFormattedResponse(name, nameSpace);

        public Stream GetStream()
        {
            var writer = new MemoryStream();
            Save(writer);
            writer.Seek(0, SeekOrigin.Begin);

            return writer;
        }
    }
}
