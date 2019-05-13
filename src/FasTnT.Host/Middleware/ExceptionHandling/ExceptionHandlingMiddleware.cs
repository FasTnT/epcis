using System;
using System.Threading.Tasks;
using FasTnT.Formatters.Xml;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FasTnT.Host.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        const int BadRequest = 400, InternalServerError = 500;
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
                if (ex is ContentTypeException)
                {
                    context.Request.Headers["Accept"] = XmlFormatterFactory.ContentTypes[0];
                }

                _logger.LogError($"[{context.TraceIdentifier}] Request failed with reason '{ex.Message}'");
                var epcisException = ex as EpcisException;
                var response = new ExceptionResponse
                {
                    Exception = (epcisException?.ExceptionType ?? ExceptionType.ImplementationException).DisplayName,
                    Severity = epcisException?.Severity ?? ExceptionSeverity.Error,
                    Reason = GetMessage(ex)
                };

                await context.SetEpcisResponse(response, (ex is EpcisException) ? BadRequest : InternalServerError, context.RequestAborted);
            }
        }

        private string GetMessage(Exception ex) => ex is EpcisException || _developmentMode ? ex.Message : "An unexpected error occured.";
    }
}
