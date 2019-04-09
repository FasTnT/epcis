using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Formatters
{
    public interface IFormatter<T>
    {
        Task<T> Read(Stream input, CancellationToken cancellationToken);
        Task Write(T entity, Stream output, CancellationToken cancellationToken);
    }
}
