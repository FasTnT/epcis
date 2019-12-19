using FasTnT.Domain.Commands;
using FasTnT.Parsers.Xml;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace FasTnT.Host.Infrastructure.Attributes
{
    public class FormatterAttribute : Attribute, IFilterFactory
    {
        public static IDictionary<Format, ICommandFormatter> KnownFormatters = new Dictionary<Format, ICommandFormatter>
        {
            { Format.Xml, new XmlCommandFormatter() },
            { Format.Soap, new SoapCommandFormatter() }
        };

        public FormatterAttribute(Format type) => Formatter = GetFormatter(type);

        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => new FormatterResourceFilter(Formatter);
        public ICommandFormatter Formatter { get; private set; }

        private ICommandFormatter GetFormatter(Format type)
        {
            if (KnownFormatters.TryGetValue(type, out ICommandFormatter formatter))
            {
                return formatter;
            }
            else
            {
                throw new ArgumentException($"Unknown EPCIS format: '{type}'");
            }
        }

        private class FormatterResourceFilter : IResourceFilter
        {
            private readonly ICommandFormatter _formatter;

            public FormatterResourceFilter(ICommandFormatter formatter) => _formatter = formatter;

            public void OnResourceExecuted(ResourceExecutedContext context) { }
            public void OnResourceExecuting(ResourceExecutingContext context) => context.HttpContext.SetFormatter(_formatter);
        }
    }
}
