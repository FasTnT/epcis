using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FasTnT.Host.Infrastructure.Attributes
{
    public class DevelopmentOnlyFilter : IActionFilter
    {
        private readonly IHostingEnvironment _environment;

        public DevelopmentOnlyFilter(IHostingEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_environment.IsDevelopment())
            {
                context.Result = new NotFoundResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
