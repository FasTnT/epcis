using FasTnT.Model.Responses;

namespace FasTnT.Formatters
{
    public interface IResponseFormatter : IFormatter<IEpcisResponse>
    {
        string ToContentTypeString();
    }
}
