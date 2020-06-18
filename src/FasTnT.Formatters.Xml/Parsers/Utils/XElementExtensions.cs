using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Utils
{
    public static class XElementExtensions
    {
        public static void AddIfAny(this XElement root, IEnumerable<XElement> elements)
        {
            if (elements != null && elements.Any())
            {
                root.Add(elements);
            }
        }

        public static void AddIfNotNull(this XElement root, XElement element)
        {
            if (element != default(XElement) && !element.IsEmpty)
            {
                root.Add(element);
            }
        }
    }
}
