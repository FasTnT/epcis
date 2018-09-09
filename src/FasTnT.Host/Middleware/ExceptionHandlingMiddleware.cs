using System;
using System.IO;
using System.Threading.Tasks;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FasTnT.Host.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                var response = new ExceptionResponse
                {
                    Exception = (ex is EpcisException epcisException) ? epcisException.ExceptionType.DisplayName : ExceptionType.ImplementationException.DisplayName,
                    Reason = ex.Message
                };

                var formatter = new EpcisResponseOutputFormatter();
                await formatter.WriteAsync(new OutputFormatterWriteContext(context, (s, e) => new StreamWriter(s, e), typeof(ExceptionResponse), response));
            }
        }
    }
}
