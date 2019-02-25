using System;

namespace FasTnT.Formatters.Soap
{
    public class SoapFormatterFactory : IFormatterFactory
    {
        public string[] AllowedContentTypes => new[] { "text/soap+xml", "application/soap+xml" };
        public IRequestFormatter RequestFormatter => throw new NotImplementedException(); // No Capture can be made using SOAP - only queries
        public IQueryFormatter QueryFormatter => new SoapQueryFormatter();
        public IResponseFormatter ResponseFormatter => new SoapResponseFormatter();
    }
}
