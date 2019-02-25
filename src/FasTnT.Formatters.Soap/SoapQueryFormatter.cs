using System;
using System.IO;
using System.Xml.Linq;
using FasTnT.Model.Queries;

namespace FasTnT.Formatters.Soap
{
    internal class SoapQueryFormatter : IQueryFormatter
    {
        static string SoapEnvelopNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
        public EpcisQuery Read(Stream input)
        {
            var document = XDocument.Load(input);
            var body = document.Element(XName.Get("Envelope", SoapEnvelopNamespace))?.Element(XName.Get("Body", SoapEnvelopNamespace));

            if(body == null || !body.HasElements)
            {
                throw new Exception("Malformed SOAP request");
            }

            return ReadRequest(body);
        }

        private EpcisQuery ReadRequest(XElement body)
        {
            throw new NotImplementedException();
        }

        public void Write(EpcisQuery entity, Stream output) => throw new NotImplementedException();
    }
}
