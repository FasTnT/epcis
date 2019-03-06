namespace FasTnT.Formatters.Json
{
    public class JsonFormatterFactory : IFormatterFactory
    {
        public string[] AllowedContentTypes => new [] { "application/json" };

        public IRequestFormatter RequestFormatter => new JsonRequestFormatter();
        public IQueryFormatter QueryFormatter => new JsonQueryFormatter();
        public IResponseFormatter ResponseFormatter => new JsonResponseFormatter();
    }
}