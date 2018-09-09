using System.Xml;

namespace FasTnT.Formatters.Xml
{
    public static class EpcisNamespaces
    {
        public static XmlNamespaceManager Manager { get; private set; } = new XmlNamespaceManager(new NameTable());

        static EpcisNamespaces()
        {
            Manager.AddNamespace("query", Query);
            Manager.AddNamespace("capture", Capture);
        }

        public const string Query = "urn:epcglobal:epcis-query:xsd:1";
        public const string Capture = "urn:epcglobal:epcis:xsd:1";
    }
}
