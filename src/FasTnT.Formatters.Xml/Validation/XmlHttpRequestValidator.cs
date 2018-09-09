using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using MoreLinq;

namespace FasTnT.Formatters.Xml.Validation
{
    public class XmlHttpRequestValidator : IRequestValidator
    {
        public static XmlHttpRequestValidator Instance { get; } = new XmlHttpRequestValidator();
        private readonly XmlSchemaSet _schema;

        private XmlHttpRequestValidator()
        {
            _schema = new XmlSchemaSet();

            var assembly = Assembly.GetExecutingAssembly();
            assembly.GetManifestResourceNames()
                    .Where(x => x.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase))
                    .Select(x => XmlReader.Create(assembly.GetManifestResourceStream(x)))
                    .ForEach(x => _schema.Add(null, x));

            _schema.Compile();
        }

        public void Validate(Stream input)
        {
            var document = XDocument.Load(new StreamReader(input));

            document.Validate(_schema, (e, t) => { if (t.Exception != null) throw t.Exception; });
        }
    }
}
