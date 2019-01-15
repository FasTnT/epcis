using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FasTnT.Host.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class DevelopmentOnlyAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => serviceProvider.GetService(typeof(DevelopmentOnlyFilter)) as IFilterMetadata;
    }
}
