using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Formatters.Soap.Responses;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using FasTnT.Model.Responses;

namespace FasTnT.Formatters.Soap
{
    internal class SoapResponseFormatter : IResponseFormatter
    {
        private const SaveOptions Options = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;
        public IEpcisResponse Read(Stream input) => throw new NotImplementedException();

        public void Write(IEpcisResponse entity, Stream output)
        {
            if (entity != default(IEpcisResponse))
            {
                FormatSoap((dynamic)entity).Save(output, Options);
            }
        }

        private XDocument FormatSoap(dynamic entity)
        {
            var document = new XDocument(new XElement(XName.Get("Envelope", "http://schemas.xmlsoap.org/soap/envelope/"), 
                new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XAttribute(XNamespace.Xmlns + "epcisq", "urn:epcglobal:epcis-query:xsd:1")
            )); 
            document.Root.Add(new XElement(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/"), Format(entity)));

            return document;
        }

        public string ToContentTypeString() => "application/soap+xml";
        public XElement Format(PollResponse response)
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

        public XElement Format(GetVendorVersionResponse response)
        {
            return new XElement(XName.Get("GetVendorVersionResult", EpcisNamespaces.Query), response.Version);
        }

        public XElement Format(GetStandardVersionResponse response)
        {
            return new XElement(XName.Get("GetStandardVersionResult", EpcisNamespaces.Query), response.Version);
        }

        public XElement Format(GetQueryNamesResponse response)
        {
            return new XElement(XName.Get("GetQueryNamesResult", EpcisNamespaces.Query), response.QueryNames.Select(x => new XElement("string", x)));
        }

        public XElement Format(GetSubscriptionIdsResult response)
        {
            return new XElement(XName.Get("GetSubscriptionIDsResult", EpcisNamespaces.Query), response.SubscriptionIds?.Select(x => new XElement("string", x)));
        }

        public XElement Format(ExceptionResponse response)
        {
            return new XElement(response.Exception, !string.IsNullOrEmpty(response.Reason) ? new XElement("reason", response.Reason) : null, (response.Severity != null) ? new XElement("severity", response.Severity.DisplayName) : null);
        }
    }
}
