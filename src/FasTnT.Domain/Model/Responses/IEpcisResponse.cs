using FasTnT.Domain.Formatter;

namespace FasTnT.Domain.Model.Responses
{
    public interface IEpcisResponse
    {
        void Accept(IEpcisResponseFormatter formatter);
    }
}
