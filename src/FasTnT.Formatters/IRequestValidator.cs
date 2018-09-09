using System.IO;

namespace FasTnT.Formatters
{
    public interface IRequestValidator
    {
        void Validate(Stream input);
    }
}
