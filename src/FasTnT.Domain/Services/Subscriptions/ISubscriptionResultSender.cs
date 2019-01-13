using FasTnT.Formatters;
using FasTnT.Model.Responses;
using System.Net;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services.Subscriptions
{
    public interface ISubscriptionResultSender
    {
        Task Send(string destination, IEpcisResponse response);
    }

    public class HttpSubscriptionResultSender : ISubscriptionResultSender
    {
        private readonly IResponseFormatter _responseFormatter;

        public HttpSubscriptionResultSender(IResponseFormatter responseFormatter)
        {
            _responseFormatter = responseFormatter;
        }

        public async Task Send(string destination, IEpcisResponse epcisResponse)
        {
            var request = WebRequest.Create(destination);
            request.ContentType = _responseFormatter.ToContentTypeString();

            using (var stream = await request.GetRequestStreamAsync())
            {
                _responseFormatter.Write(epcisResponse, stream);
            }

            var response = await request.GetResponseAsync();
        }
    }
}
