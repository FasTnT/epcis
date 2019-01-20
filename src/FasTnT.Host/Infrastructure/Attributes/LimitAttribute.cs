using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FasTnT.Host.Infrastructure.Attributes
{
    public class LimitAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => serviceProvider.GetService(typeof(LimitFilter)) as IFilterMetadata;
    }

    internal class TooManyRequestsResult : ActionResult
    {
        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 429;
        }
    }
}
