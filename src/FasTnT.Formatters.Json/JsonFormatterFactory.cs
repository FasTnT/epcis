using System;

namespace FasTnT.Formatters.Json
{
    public class JsonFormatterFactory : IFormatterFactory
    {
        public string[] AllowedContentTypes => new [] { "application/json" };

        public IRequestFormatter RequestFormatter => new JsonRequestFormatter();
        public IQueryFormatter QueryFormatter => throw new NotImplementedException($"JSON content-type is not supported for queries");
        public IResponseFormatter ResponseFormatter => new JsonResponseFormatter();
    }
}