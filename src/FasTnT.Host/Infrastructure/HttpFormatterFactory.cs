using FasTnT.Formatters;
using FasTnT.Formatters.Soap;
using FasTnT.Formatters.Xml;
using FasTnT.Model;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
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
                new SoapFormatterFactory()
            };
        }

        public IFormatter<T> GetFormatter<T>(HttpContext httpContext)
        {
            var factory = _formatters.FirstOrDefault(x => x.AllowedContentTypes.Contains(httpContext.Request.ContentType, StringComparer.OrdinalIgnoreCase));

            return (IFormatter<T>) GetFormatter(typeof(T), factory);
        }

        private object GetFormatter(Type type, IFormatterFactory factory)
        {
            if (type == typeof(Request)) return factory?.RequestFormatter;
            if (type == typeof(EpcisQuery)) return factory?.QueryFormatter;
            if (type == typeof(IEpcisResponse)) return factory?.ResponseFormatter;

            return null;
        }
    }
}
