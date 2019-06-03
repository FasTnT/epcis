using System;
using FasTnT.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FasTnT.Host.Controllers
{
    public class SoapFormatterAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => SoapFormatterResourceFilter.Instance;
    }

    public class SoapFormatterResourceFilter : IResourceFilter
    {
        public static IResourceFilter Instance = new SoapFormatterResourceFilter();

        public void OnResourceExecuted(ResourceExecutedContext context) { }
        public void OnResourceExecuting(ResourceExecutingContext context) => context.HttpContext.SetFormatter(SoapFormatter.Instance);
    }
}
