using FasTnT.Domain;
using FasTnT.Model;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Host.Infrastructure
{
    public class RequestBodyFormatter : InputFormatter
    {
        public RequestBodyFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
        }

        public override bool CanRead(InputFormatterContext context) => context?.ModelType == typeof(CaptureRequest);

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;
            var formatterProvider = context.HttpContext.RequestServices.GetService<FormatterProvider>();
            var formatter = formatterProvider.GetFormatter<Request>(contentType);

            return await InputFormatterResult.SuccessAsync(await formatter.Read(request.Body, context.HttpContext.RequestAborted));
        }
    }
}