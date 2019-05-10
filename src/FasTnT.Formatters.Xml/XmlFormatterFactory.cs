namespace FasTnT.Formatters.Xml
{
    public class XmlFormatterFactory : IFormatterFactory
    {
        public static string[] ContentTypes = new[] { "text/xml", "application/xml" };

        public string[] AllowedContentTypes { get; } = ContentTypes;
        public IRequestFormatter RequestFormatter => new XmlRequestFormatter();
        public IQueryFormatter QueryFormatter => new XmlQueryFormatter();
        public IResponseFormatter ResponseFormatter => new XmlResponseFormatter();
    }
}
