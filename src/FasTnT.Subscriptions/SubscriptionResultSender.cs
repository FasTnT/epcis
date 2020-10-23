using FasTnT.Commands.Responses;
using FasTnT.Parsers.Xml;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Subscriptions
{
    public class SubscriptionResultSender
    {
        public async Task Send(string destination, IEpcisResponse epcisResponse, CancellationToken cancellationToken)
        {
            var request = WebRequest.CreateHttp(destination);
            request.Method = "POST";
            TrySetBasicAuthorization(request);

            await WriteRequestPayload(request, epcisResponse, cancellationToken);
            await EnsureResponseIsSuccessful(request, cancellationToken);
        }

        private async Task EnsureResponseIsSuccessful(HttpWebRequest request, CancellationToken cancellationToken)
        {
            using var registration = cancellationToken.Register(() => request.Abort(), false);
            using var response = await request.GetResponseAsync() as HttpWebResponse;
            using var responseMessage = new HttpResponseMessage(response.StatusCode);

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new Exception($"Response does not indicate success status code: {response.StatusCode} ({response.StatusDescription})");
            }
        }

        private async Task WriteRequestPayload(HttpWebRequest request, IEpcisResponse epcisResponse, CancellationToken cancellationToken)
        {
            using var stream = await request.GetRequestStreamAsync();
            var formatter = new XmlCommandFormatter();

            request.ContentType = formatter.ContentType;
            await formatter.WriteResponse(epcisResponse, stream, cancellationToken);
        }

        private void TrySetBasicAuthorization(HttpWebRequest request)
        {
            if (!string.IsNullOrEmpty(request.RequestUri.UserInfo))
            {
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(WebUtility.UrlDecode(request.RequestUri.UserInfo)));
                request.Headers.Add("Authorization", $"Basic {token}");
            }
        }
    }
}
