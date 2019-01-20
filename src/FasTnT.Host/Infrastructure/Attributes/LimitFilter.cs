using FasTnT.Domain.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FasTnT.Host.Infrastructure.Attributes
{
    public class LimitFilter : IActionFilter
    {
        private readonly UserService _userService;

        public LimitFilter(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(!_userService.CanMakeRequest(_userService.Current))
            {
                context.Result = new TooManyRequestsResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
