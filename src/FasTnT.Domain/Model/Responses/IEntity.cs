using FasTnT.Domain.Formatter;

namespace FasTnT.Domain.Model.Responses
{
    public interface IEntity
    {
        void Accept(IEventFormatter formatter);
    }
}
