using System;

namespace FasTnT.Formatters.Xml
{
    public class SoapFormatter : IFormatterFactory
    {
        private SoapFormatter() { }
        public static IFormatterFactory Instance = new SoapFormatter();

        public string ContentType { get { return "text/xml"; } }
        public IRequestFormatter RequestFormatter => throw new NotImplementedException();
        public IQueryFormatter QueryFormatter => new SoapQueryFormatter();
        public IResponseFormatter ResponseFormatter => new SoapResponseFormatter();
    }
}
