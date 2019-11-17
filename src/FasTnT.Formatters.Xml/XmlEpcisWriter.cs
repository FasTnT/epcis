using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml
{
    public abstract class XmlEpcisWriter<T>
    {
        public abstract Task Write(T entity, Stream output, CancellationToken cancellationToken);

        internal async Task Write(T entity, Stream output, Func<dynamic, XDocument> write, CancellationToken cancellationToken)
        {
            XDocument document = write((dynamic)entity);
            var bytes = Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces));

            await output.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }
    }
}
