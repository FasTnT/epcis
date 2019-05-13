using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Extensions;

namespace FasTnT.Host
{
    internal class EpcisMigrationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _path;

        public EpcisMigrationMiddleware(RequestDelegate next, string path)
        {
            _next = next;
            _path = path;
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var unitOfWork = serviceProvider.GetService<IUnitOfWork>();

            if (HttpMethods.IsPost(httpContext.Request.Method) && httpContext.Request.Path.Value.Equals($"{_path}/Migrate", StringComparison.OrdinalIgnoreCase))
            {
                await unitOfWork.Execute(async uow => await uow.DatabaseManager.Migrate(httpContext.RequestAborted));
            }
            else if (HttpMethods.IsPost(httpContext.Request.Method) && httpContext.Request.Path.Value.Equals($"{_path}/Rollback", StringComparison.OrdinalIgnoreCase))
            {
                await unitOfWork.Execute(async uow => await uow.DatabaseManager.Rollback(httpContext.RequestAborted));
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }
}
