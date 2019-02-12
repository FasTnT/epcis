using FasTnT.Formatters;
using FasTnT.Model.Responses;
using System.Net;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Subscriptions
{
    public class HttpSubscriptionResultSender : ISubscriptionResultSender
    {
        private readonly IResponseFormatter _responseFormatter;

        public HttpSubscriptionResultSender(IResponseFormatter responseFormatter)
        {
            _responseFormatter = responseFormatter;
        }

        public async Task Send(string destination, IEpcisResponse epcisResponse)
        {
            var request = WebRequest.Create($"{destination}{GetCallbackUrl(epcisResponse)}");
            request.Method = "POST";
            request.ContentType = _responseFormatter.ToContentTypeString();

            using (var stream = await request.GetRequestStreamAsync())
            {
                _responseFormatter.Write(epcisResponse, stream);
            }

            var response = await request.GetResponseAsync();
        }

        private string GetCallbackUrl(IEpcisResponse response)
        {
            if(response is PollResponse) return "CallbackResults";
            if ((response is ExceptionResponse res) && res.Exception == "QueryTooLargeException") return "CallbackQueryTooLargeException";

            return "CallbackImplementationException"; // In every other case.
        }
    }
}
