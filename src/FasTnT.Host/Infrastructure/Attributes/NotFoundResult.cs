using Microsoft.AspNetCore.Mvc;

namespace FasTnT.Host.Infrastructure.Attributes
{
    internal class NotFoundResult : ActionResult
    {
        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 404;
        }
    }
}
