using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using FasTnT.Model.Exceptions;
using MoreLinq;

namespace FasTnT.Formatters.Xml.Validation
{
    public class XmlDocumentParser
    {
        public static XmlDocumentParser Instance { get; } = new XmlDocumentParser();
        private readonly XmlSchemaSet _schema;

        private XmlDocumentParser()
        {
            _schema = new XmlSchemaSet();

            var assembly = Assembly.GetExecutingAssembly();
            assembly.GetManifestResourceNames()
                    .Where(x => x.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase))
                    .Select(x => XmlReader.Create(assembly.GetManifestResourceStream(x)))
                    .ForEach(x => _schema.Add(null, x));

            _schema.Compile();
        }

        public XDocument Load(Stream input)
        {
            var document = XDocument.Load(input);
            document.Validate(_schema, (e, t) => { if (t.Exception != null) throw new EpcisException(ExceptionType.ValidationException, t.Exception.Message); });

            return document;
        }
    }
}
