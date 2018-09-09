using System.IO;
using System.Text;

namespace FasTnT.Domain.Model.Responses.Formatted.Json
{
    public class JsonFormattedResponse : IFormattedResponse
    {
        private readonly string _payload;

        public JsonFormattedResponse(string payload)
        {
            _payload = payload;
        }

        public Stream GetStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_payload));
        }
    }
}
