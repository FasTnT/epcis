using System;
using FasTnT.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FasTnT.Host.Controllers
{
    public class XmlFormatterAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => XmlFormatterResourceFilter.Instance;
    }

    public class XmlFormatterResourceFilter : IResourceFilter
    {
        public static IFilterMetadata Instance = new XmlFormatterResourceFilter();

        public void OnResourceExecuted(ResourceExecutedContext context) { }
        public void OnResourceExecuting(ResourceExecutingContext context) => context.HttpContext.SetFormatter(XmlFormatter.Instance);
    }
}
