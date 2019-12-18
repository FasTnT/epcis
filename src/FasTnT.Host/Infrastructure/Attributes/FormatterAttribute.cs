using FasTnT.Domain.Commands;
using FasTnT.Parsers.Xml;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FasTnT.Host.Infrastructure.Attributes
{
    public class FormatterAttribute : Attribute, IFilterFactory
    {
        public FormatterAttribute(Format type) => Formatter = GetFormatter(type);

        public ICommandParser Formatter { get; private set; }
        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => new FormatterResourceFilter(Formatter);

        private ICommandParser GetFormatter(Format type)
        {
            return new XmlCommandParser();
        }

        private class FormatterResourceFilter : IResourceFilter
        {
            private readonly ICommandParser _formatter;

            public FormatterResourceFilter(ICommandParser formatter) => _formatter = formatter;

            public void OnResourceExecuted(ResourceExecutedContext context) { }
            public void OnResourceExecuting(ResourceExecutingContext context) => context.HttpContext.SetFormatter(_formatter);
        }
    }
}
