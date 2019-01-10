using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FasTnT.Formatters.Xml.Requests;
using FasTnT.Formatters.Xml.Validation;
using FasTnT.Model.Queries;

namespace FasTnT.Formatters.Xml
{
    public class XmlQueryFormatter : IQueryFormatter
    {
        public EpcisQuery Read(Stream input)
        {
            var document = XmlDocumentParser.Instance.Load(input);

            if (document.Root.Name.LocalName == "EPCISQueryDocument")
            {
                var element = document.Root.Element("EPCISBody").Elements().FirstOrDefault();

                if (element != null)
                {
                    if (element.Name == XName.Get("GetQueryNames", EpcisNamespaces.Query))
                    {
                        return new GetQueryNames();
                    }
                    if (element.Name == XName.Get("GetSubscriptionIDs", EpcisNamespaces.Query))
                    {
                        return new GetSubscriptionIds();
                    }
                    if (element.Name == XName.Get("GetStandardVersion", EpcisNamespaces.Query))
                    {
                        return new GetStandardVersion();
                    }
                    if (element.Name == XName.Get("GetVendorVersion", EpcisNamespaces.Query))
                    {
                        return new GetVendorVersion();
                    }
                    if (element.Name == XName.Get("Poll", EpcisNamespaces.Query))
                    {
                        return XmlQueryParser.Parse(element);
                    }
                    throw new Exception($"Element not expected: '{element?.Name?.LocalName ?? null}'");
                }

                throw new Exception($"EPCISBody element must contain the query type.");
            }

            throw new Exception($"Element not expected: '{document.Root.Name.LocalName}'");
        }

        public void Write(EpcisQuery entity, Stream output)
        {
            XDocument document = Write((dynamic)entity);
            var bytes = Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces)); 

            output.Write(bytes, 0, bytes.Length);
        }

        private XDocument Write(GetQueryNames query) => Query(XName.Get("GetQueryNames", EpcisNamespaces.Query));
        private XDocument Write(GetSubscriptionIds query) => Query(XName.Get("GetSubscriptionIDs", EpcisNamespaces.Query));
        private XDocument Write(GetStandardVersion query) => Query(XName.Get("GetStandardVersion", EpcisNamespaces.Query));
        private XDocument Write(GetVendorVersion query) => Query(XName.Get("GetVendorVersion", EpcisNamespaces.Query));
        private XDocument Write(PredefinedQuery query) => throw new NotImplementedException();

        private XDocument Query(XName queryName) => new XDocument(XName.Get("EPCISQueryDocument", EpcisNamespaces.Query), new XElement("EPCISBody", new XElement(queryName)));
    }
}
