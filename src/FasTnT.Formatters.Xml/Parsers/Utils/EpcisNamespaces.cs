using System.Xml;

namespace FasTnT.Parsers.Xml.Utils
{
    public static class EpcisNamespaces
    {
        public static XmlNamespaceManager Manager { get; private set; } = new XmlNamespaceManager(new NameTable());

        static EpcisNamespaces()
        {
            Manager.AddNamespace("query", Query);
            Manager.AddNamespace("capture", Capture);
            Manager.AddNamespace("masterdata", MasterData);
            Manager.AddNamespace("sbdh", SBDH);
        }

        public const string Query = "urn:epcglobal:epcis-query:xsd:1";
        public const string Capture = "urn:epcglobal:epcis:xsd:1";
        public const string MasterData = "urn:epcglobal:epcis-masterdata:xsd:1";
        public const string SBDH = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader";
    }
}
