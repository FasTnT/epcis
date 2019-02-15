using System;
using System.IO;
using System.Threading.Tasks;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FasTnT.Host.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        const int BadRequest = 400, InternalServerError = 500;
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        } 

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                _logger.LogError($"[{context.TraceIdentifier}] Request failed with reason '{ex.Message}'");
                var epcisException = ex as EpcisException;
                var response = new ExceptionResponse
                {
                    Exception = epcisException?.ExceptionType?.DisplayName ?? ExceptionType.ImplementationException.DisplayName,
                    Reason = ex.Message,
                    Severity = epcisException == null ? ExceptionSeverity.Error : epcisException.Severity 
                };

                context.Response.StatusCode = (ex is EpcisException) ? BadRequest : InternalServerError;
                var formatter = new EpcisResponseOutputFormatter();
                await formatter.WriteAsync(new OutputFormatterWriteContext(context, (s, e) => new StreamWriter(s, e), typeof(ExceptionResponse), response));
            }
        }
    }
}
