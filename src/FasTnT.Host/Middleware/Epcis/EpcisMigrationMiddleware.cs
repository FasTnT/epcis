using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Extensions;

namespace FasTnT.Host
{
    internal class EpcisMigrationMiddleware
    {
        private readonly ILogger<EpcisMigrationMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly string _path;

        public EpcisMigrationMiddleware(ILogger<EpcisMigrationMiddleware> logger, RequestDelegate next, string path)
        {
            _logger = logger;
            _next = next;
            _path = path;
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var unitOfWork = serviceProvider.GetService<IUnitOfWork>();

            if (httpContext.Request.Method == "POST" && httpContext.Request.Path.Value.Equals($"{_path}/Migrate", StringComparison.OrdinalIgnoreCase))
            {
                await unitOfWork.Execute(async uow => await uow.DatabaseManager.Migrate());
            }
            else if (httpContext.Request.Method == "POST" && httpContext.Request.Path.Value.Equals($"{_path}/Rollback", StringComparison.OrdinalIgnoreCase))
            {
                await unitOfWork.Execute(async uow => await uow.DatabaseManager.Rollback());
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }
}
