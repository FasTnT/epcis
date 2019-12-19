using System;
using System.Net;
using System.Threading.Tasks;
using FasTnT.Commands.Responses;
using FasTnT.Model.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FasTnT.Host.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly bool _developmentMode;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, bool developmentMode)
        {
            _next = next;
            _logger = logger;
            _developmentMode = developmentMode;
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

                context.Response.ContentType = context.Request.ContentType;
                context.Response.StatusCode = (int)(ex is EpcisException ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError);

                var response = new ExceptionResponse
                {
                    Exception = (epcisException?.ExceptionType ?? ExceptionType.ImplementationException).DisplayName,
                    Severity = epcisException?.Severity ?? ExceptionSeverity.Error,
                    Reason = GetMessage(ex)
                };

                await context.GetFormatter().WriteResponse(response, context.Response.Body, context.RequestAborted);
            }
        }

        private string GetMessage(Exception ex) => ex is EpcisException || _developmentMode ? ex.Message : "An unexpected error occured.";
    }
}
