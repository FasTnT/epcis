using FasTnT.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FasTnT.Host.Controllers
{
    public class JsonFormatterAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => JsonFormatterResourceFilter.Instance;

        private class JsonFormatterResourceFilter : IResourceFilter
        {
            public static IFilterMetadata Instance = new JsonFormatterResourceFilter();

            public void OnResourceExecuted(ResourceExecutedContext context) { }
            public void OnResourceExecuting(ResourceExecutingContext context) => context.HttpContext.SetFormatter(XmlFormatter.Instance);
        }
    }
}
