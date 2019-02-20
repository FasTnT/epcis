using FasTnT.Formatters;
using FasTnT.Formatters.Xml;
using FasTnT.Model;
using FasTnT.Model.Queries;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace FasTnT.Host.Infrastructure
{
    public class HttpFormatterFactory
    {
        public static HttpFormatterFactory Instance { get; } = new HttpFormatterFactory();

        IDictionary<string, IDictionary<Type, object>> _knownTypes = new Dictionary<string, IDictionary<Type, object>>
        {
            { "application/xml", new Dictionary<Type, object>
                {
                    { typeof(Request), new XmlRequestFormatter() },
                    { typeof(EpcisQuery), new XmlQueryFormatter() },
                }
            }
        };

        private HttpFormatterFactory() { }

        public IFormatter<T> GetFormatter<T>(HttpContext httpContext)
        {
            return _knownTypes["application/xml"][typeof(T)] as IFormatter<T>;
        }
    }
}
