using FasTnT.Commands.Responses;
using FasTnT.Parsers.Xml.Utils;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters
{
    public class XmlResponseFormatter : BaseXmlFormatter
    {
        public override XDocument FormatInternal(PollResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(new XElement("EPCISBody", FormatPollResponse(response)));

            return formatted;
        }

        public override XDocument FormatInternal(GetVendorVersionResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(new XElement("EPCISBody", new XElement(XName.Get("GetVendorVersionResult", EpcisNamespaces.Query), response.Version)));

            return formatted;
        }

        public override XDocument FormatInternal(GetSubscriptionIdsResponse response)
        {
            return CreateQueryResponse(XName.Get("GetSubscriptionIDsResult", EpcisNamespaces.Query), response.SubscriptionIds.Select(x => new XElement("string", x)));
        }

        public override XDocument FormatInternal(GetStandardVersionResponse response)
        {
            return CreateQueryResponse(XName.Get("GetStandardVersionResult", EpcisNamespaces.Query), response.Version);
        }

        public override XDocument FormatInternal(GetQueryNamesResponse response)
        {
            return CreateQueryResponse(XName.Get("GetQueryNamesResult", EpcisNamespaces.Query), response.QueryNames.Select(x => new XElement("string", x)));
        }

        private XDocument CreateQueryResponse(XName element, object content)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(new XElement("EPCISBody", new XElement(element, content)));

            return formatted;
        }

        public override XDocument FormatInternal(ExceptionResponse response)
        {
            var formatted = CreateResponse(response.Exception, false);
            formatted.Root.Add(!string.IsNullOrEmpty(response.Reason) ? new XElement("reason", response.Reason) : null);
            if (response.Severity != null) formatted.Root.Add(new XElement("severity", response.Severity.DisplayName));

            return formatted;
        }

        public static XDocument CreateResponse(string name, bool withAttributes = true) => new XDocument(new XElement(XName.Get(name, EpcisNamespaces.Query), new XAttribute(XNamespace.Xmlns + "epcisq", EpcisNamespaces.Query), withAttributes ? Attributes() : null));
        public static XAttribute[] Attributes() => new[] { new XAttribute("creationDate", DateTime.UtcNow), new XAttribute("schemaVersion", "1") };
    }
}
