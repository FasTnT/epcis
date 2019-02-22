namespace FasTnT.Formatters.Xml
{
    public class XmlFormatterFactory : IFormatterFactory
    {
        public string[] AllowedContentTypes => new[] { "text/xml", "application/xml" };
        public IRequestFormatter RequestFormatter => new XmlRequestFormatter();
        public IQueryFormatter QueryFormatter => new XmlQueryFormatter();
        public IResponseFormatter ResponseFormatter => new XmlResponseFormatter();
    }
}
