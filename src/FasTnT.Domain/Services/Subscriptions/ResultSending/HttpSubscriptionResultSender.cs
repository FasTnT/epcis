using FasTnT.Formatters.Xml;
using FasTnT.Model.Responses;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Subscriptions
{
    public class HttpSubscriptionResultSender : ISubscriptionResultSender
    {
        public async Task Send(string destination, IEpcisResponse epcisResponse, string contentType, CancellationToken cancellationToken)
        {
            var responseFormatter = new XmlResponseFormatter();
            var request = WebRequest.CreateHttp($"{destination}{GetCallbackUrl(epcisResponse)}");
            request.Method = "POST";
            request.ContentType = responseFormatter.ToContentTypeString();
            TrySetAuthorization(request);

            using (var stream = await request.GetRequestStreamAsync())
            {
                await responseFormatter.Write(epcisResponse, stream, cancellationToken);
            }

            var response = await request.GetResponseAsync() as HttpWebResponse;

            if(!new HttpResponseMessage(response.StatusCode).IsSuccessStatusCode)
            {
                throw new Exception($"Response does not indicate success status code: {response.StatusCode} ({response.StatusDescription})");
            }
        }

        private void TrySetAuthorization(HttpWebRequest request)
        {
            if (!string.IsNullOrEmpty(request.RequestUri.UserInfo))
            {
                request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(WebUtility.UrlDecode(request.RequestUri.UserInfo)))}");
            }
        }

        private string GetCallbackUrl(IEpcisResponse response)
        {
            if(response is PollResponse) return "CallbackResults";
            if ((response is ExceptionResponse res) && res.Exception == "QueryTooLargeException") return "CallbackQueryTooLargeException";

            return "CallbackImplementationException"; // In every other case.
        }
    }
}
