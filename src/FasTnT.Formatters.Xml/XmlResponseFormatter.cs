using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using FasTnT.Model.Responses;

namespace FasTnT.Formatters.Xml
{
    public class XmlResponseFormatter : IResponseFormatter
    {
        private const SaveOptions Options = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;
        public IEpcisResponse Read(Stream input) => throw new NotImplementedException();

        public void Write(IEpcisResponse entity, Stream output)
        {
            if (entity != default(IEpcisResponse))
            {
                Format((dynamic)entity).Save(output, Options);
            }
        }

        public string ToContentTypeString() => "application/xml";
        public XDocument Format(PollResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            var resultName = "EventList";
            var typeOfResponse = response.Entities.GetType().GenericTypeArguments[0];

            if (typeOfResponse == typeof(EpcisEvent)) resultName = "EventList";
            if (typeOfResponse == typeof(EpcisMasterData)) resultName = "VocabularyList";

            formatted.Root.Add(
                new XElement("EPCISBody",
                    new XElement(XName.Get("QueryResults", EpcisNamespaces.Query),
                        new XElement("queryName", response.QueryName),
                        !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                        new XElement("resultsBody", new XElement(resultName, XmlEntityFormatter.Format(response.Entities)))
                    )
                )
            );

            return formatted;
        }

        public XDocument Format(GetVendorVersionResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(new XElement("EPCISBody", new XElement(XName.Get("GetVendorVersionResult", EpcisNamespaces.Query), response.Version)));

            return formatted;
        }

        public XDocument Format(GetStandardVersionResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(new XElement("EPCISBody", new XElement(XName.Get("GetStandardVersionResult", EpcisNamespaces.Query), response.Version)));

            return formatted;
        }

        public XDocument Format(GetQueryNamesResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(
                new XElement("EPCISBody", new XElement(XName.Get("GetQueryNamesResult", EpcisNamespaces.Query), response.QueryNames.Select(x => new XElement("string", x))))
            );

            return formatted;
        }

        public XDocument Format(GetSubscriptionIdsResult response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(
                new XElement("EPCISBody", new XElement(XName.Get("GetSubscriptionIDsResult", EpcisNamespaces.Query), response.SubscriptionIds?.Select(x => new XElement("string", x))))
            );

            return formatted;
        }

        public XDocument Format(ExceptionResponse response)
        {
            var formatted = CreateResponse(response.Exception, false);
            formatted.Root.Add(!string.IsNullOrEmpty(response.Reason) ? new XElement("reason", response.Reason) : null);
            if(response.Severity != null) formatted.Root.Add(new XElement("severity", response.Severity.DisplayName));

            return formatted;
        }

        public static XDocument CreateResponse(string name, bool withAttributes = true) => new XDocument(new XElement(XName.Get(name, EpcisNamespaces.Query), new XAttribute(XNamespace.Xmlns + "epcisq", EpcisNamespaces.Query), withAttributes ? Attributes() : null));
        public static XAttribute[] Attributes() => new[] { new XAttribute("creationDate", DateTime.UtcNow), new XAttribute("schemaVersion", "1") };
    }
}
