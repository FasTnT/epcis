using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using FasTnT.Formatters.Json;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace FasTnT.Host.Infrastructure
{
    public class HttpFormatterFactory
    {
        private IFormatterFactory[] _formatters;
        public static HttpFormatterFactory Instance { get; } = new HttpFormatterFactory();

        private HttpFormatterFactory()
        {
            _formatters = new IFormatterFactory[]
            {
                new XmlFormatterFactory(),
                new SoapFormatterFactory(),
                new JsonFormatterFactory()
            };
        }

        public IFormatter<T> GetFormatter<T>(HttpContext httpContext)
        {
            var factory = _formatters.FirstOrDefault(x => x.AllowedContentTypes.Contains(httpContext.Request.ContentType, StringComparer.OrdinalIgnoreCase));

            return factory.GetFormatter<T>();
        }
    }
}
