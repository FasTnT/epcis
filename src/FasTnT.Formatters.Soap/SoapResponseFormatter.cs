using System;
using System.IO;
using FasTnT.Model.Responses;

namespace FasTnT.Formatters.Soap
{
    internal class SoapResponseFormatter : IResponseFormatter
    {
        public IEpcisResponse Read(Stream input) => throw new NotImplementedException();
        public void Write(IEpcisResponse entity, Stream output) => throw new NotImplementedException();
        public string ToContentTypeString() => "application/soap+xml";
    }
}
