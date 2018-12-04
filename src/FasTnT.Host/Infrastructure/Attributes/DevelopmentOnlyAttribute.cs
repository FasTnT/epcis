using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FasTnT.Host.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class DevelopmentOnlyAttribute : ActionFilterAttribute
    {
        public static Func<IHostingEnvironment> Configuration { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Configuration().IsDevelopment())
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new NotFoundResult();
            }
        }
    }
}
