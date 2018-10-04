using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Domain;
using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model.MasterDatas;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;

namespace FasTnT.Formatters.Xml
{
    public class XmlResponseFormatter : IResponseFormatter
    {
        private const SaveOptions Options = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;
        public IEpcisResponse Read(Stream input) => throw new NotImplementedException();

        public void Write(IEpcisResponse entity, Stream output)
        {
            Format((dynamic)entity).Save(output, Options);
        }

        public XDocument Format(PollResponse response)
        {
            var formatted = WithAttributes("EPCISQueryDocument", EpcisNamespaces.Query);
            var resultName = string.Empty;
            var typeOfResponse = response.Entities.GetType().GenericTypeArguments[0];

            if (typeOfResponse == typeof(EpcisEvent)) resultName = "EventList";
            if (typeOfResponse == typeof(MasterData)) resultName = "VocabularyList";

            formatted.Root.Add(
                new XElement("EPCISBody",
                    new XElement(XName.Get("QueryResults", EpcisNamespaces.Query),
                        new XElement("queryName", response.QueryName),
                        !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                        new XElement("resultBody", new XElement(resultName, response.Entities.Select(XmlEventFormatter.Format)))
                    )
                )
            );

            return formatted;
        }

        public XDocument Format(GetVendorVersionResponse response)
        {
            var formatted = WithAttributes("EPCISQueryDocument", EpcisNamespaces.Query);
            formatted.Root.Add(new XElement("EPCISBody", new XElement("GetVendorVersionResult", response.Version)));

            return formatted;
        }

        public XDocument Format(GetStandardVersionResponse response)
        {
            var formatted = WithAttributes("EPCISQueryDocument", EpcisNamespaces.Query);
            formatted.Root.Add(new XElement("EPCISBody", new XElement("GetStandardVersionResult", response.Version)));

            return formatted;
        }

        public XDocument Format(GetQueryNamesResponse response)
        {
            var formatted = WithAttributes("EPCISQueryDocument", EpcisNamespaces.Query);
            formatted.Root.Add(
                new XElement("EPCISBody", new XElement("GetQueryNamesResult", response.QueryNames.Select(x => new XElement("string", x))))
            );

            return formatted;
        }

        public XDocument Format(GetSubscriptionIdsResult response)
        {
            var formatted = WithAttributes("EPCISQueryDocument", EpcisNamespaces.Query);
            formatted.Root.Add(
                new XElement("EPCISBody", new XElement("GetSubscriptionIDsResult", response.SubscriptionIds.Select(x => new XElement("string", x))))
            );

            return formatted;
        }

        public XDocument Format(SubscribeResponse response)
        {
            var formatted = WithAttributes("EPCISQueryDocument", EpcisNamespaces.Query);
            formatted.Root.Add(new XElement("EPCISBody", new XElement("SubscribeResult")));

            return formatted;
        }

        public XDocument Format(CaptureSucceedResponse response)
        {
            var formatted = WithoutAttributes("Result");
            formatted.Root.Add("OK");

            return formatted;
        }

        public XDocument Format(ExceptionResponse response)
        {
            var formatted = WithAttributes(response.Exception, EpcisNamespaces.Query);
            formatted.Root.Add(!string.IsNullOrEmpty(response.Reason) ? new XElement("Reason", response.Reason) : null);

            return formatted;
        }

        public static XDocument WithAttributes(string name, string nameSpace = "") => new XDocument(new XElement(XName.Get(name, nameSpace), Attributes()));
        public static XAttribute[] Attributes() => new[] { new XAttribute("creationDate", DateTime.UtcNow), new XAttribute("schemaVersion", "1.2"), new XAttribute(XNamespace.Xmlns + "epcisq", EpcisNamespaces.Query) };
        public static XDocument WithoutAttributes(string name, string nameSpace = "") => new XDocument(new XElement(XName.Get(name, nameSpace)));
        public string ToContentTypeString() => "application/xml";
    }
}
