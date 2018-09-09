using System.IO;

namespace FasTnT.Formatters
{
    public interface IFormatter<T>
    {
        T Read(Stream input);
        void Write(T entity, Stream output);
    }
}
