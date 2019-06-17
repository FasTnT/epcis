using FasTnT.Formatters;
using FasTnT.Formatters.Json;
using FasTnT.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FasTnT.Host.Infrastructure.Attributes
{
    public class FormatterAttribute : Attribute, IFilterFactory
    {
        public FormatterAttribute(Format type) => Formatter = GetFormatter(type);

        public IFormatter Formatter { get; private set; }
        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => new FormatterResourceFilter(Formatter);

        private IFormatter GetFormatter(Format type)
        {
            switch(type)
            {
                case Format.Json: return JsonFormatter.Instance;
                case Format.Soap: return SoapFormatter.Instance;
                case Format.Xml: return XmlFormatter.Instance;
                default: throw new ArgumentOutOfRangeException(type.ToString());
            };
        }

        private class FormatterResourceFilter : IResourceFilter
        {
            private readonly IFormatter _formatter;

            public FormatterResourceFilter(IFormatter formatter) => _formatter = formatter;

            public void OnResourceExecuted(ResourceExecutedContext context) { }
            public void OnResourceExecuting(ResourceExecutingContext context) => context.HttpContext.SetFormatter(_formatter);
        }
    }
}
