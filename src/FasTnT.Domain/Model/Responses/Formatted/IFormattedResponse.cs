using System.IO;

namespace FasTnT.Domain.Model.Responses.Formatted
{
    public interface IFormattedResponse
    {
        Stream GetStream();
    }
}
