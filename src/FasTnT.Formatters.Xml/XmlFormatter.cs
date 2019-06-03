namespace FasTnT.Formatters.Xml
{
    public class XmlFormatter : IFormatterFactory
    {
        private XmlFormatter() { }
        public static IFormatterFactory Instance = new XmlFormatter();

        public string ContentType { get { return "application/xml"; } }
        public IRequestFormatter RequestFormatter => new XmlRequestFormatter();
        public IQueryFormatter QueryFormatter => new XmlQueryFormatter();
        public IResponseFormatter ResponseFormatter => new XmlResponseFormatter();
    }
}
