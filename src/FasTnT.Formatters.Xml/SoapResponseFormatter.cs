using System.IO;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using FasTnT.Model.Responses;

namespace FasTnT.Formatters.Xml
{
    internal class SoapResponseFormatter : BaseResponseFormatter<XElement>
    {
        private const SaveOptions Options = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;
        public override string ToContentTypeString() => "application/soap+xml";

        public override void Write(IEpcisResponse entity, Stream output)
        {
            if (entity == default(IEpcisResponse)) return;

            FormatSoap(Format(entity)).Save(output, Options);
        }

        private XDocument FormatSoap(XElement entity)
        {
            var document = new XDocument(new XElement(XName.Get("Envelope", "http://schemas.xmlsoap.org/soap/envelope/"), 
                new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XAttribute(XNamespace.Xmlns + "epcisq", "urn:epcglobal:epcis-query:xsd:1")
            )); 
            document.Root.Add(new XElement(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/"), entity));

            return document;
        }

        protected override XElement FormatInternal(PollResponse response)
        {
            var resultName = "EventList";
            var typeOfResponse = response.Entities.GetType().GenericTypeArguments[0];

            if (typeOfResponse == typeof(EpcisEvent)) resultName = "EventList";
            if (typeOfResponse == typeof(EpcisMasterData)) resultName = "VocabularyList";

            var evt = new XElement(XName.Get("QueryResults", EpcisNamespaces.Query),
                new XElement("queryName", response.QueryName),
                !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                new XElement("resultsBody", new XElement(resultName, XmlEntityFormatter.Format(response.Entities)))
            );

            return evt;
        }

        protected override XElement FormatInternal(GetVendorVersionResponse response)
            => new XElement(XName.Get("GetVendorVersionResult", EpcisNamespaces.Query), response.Version);

        protected override XElement FormatInternal(GetStandardVersionResponse response)
            => new XElement(XName.Get("GetStandardVersionResult", EpcisNamespaces.Query), response.Version);

        protected override XElement FormatInternal(GetQueryNamesResponse response)
            => new XElement(XName.Get("GetQueryNamesResult", EpcisNamespaces.Query), response.QueryNames.Select(x => new XElement("string", x)));

        protected override XElement FormatInternal(GetSubscriptionIdsResult response)
            => new XElement(XName.Get("GetSubscriptionIDsResult", EpcisNamespaces.Query), response.SubscriptionIds?.Select(x => new XElement("string", x)));

        protected override XElement FormatInternal(ExceptionResponse response)
            => new XElement(response.Exception, !string.IsNullOrEmpty(response.Reason) ? new XElement("reason", response.Reason) : null, (response.Severity != null) ? new XElement("severity", response.Severity.DisplayName) : null);
    }
}
