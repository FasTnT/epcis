using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.Parsers.Xml.Formatters.Implementation;
using FasTnT.Parsers.Xml.Utils;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public abstract class BaseV1_2EventFormatter : BaseV1EventFormatter
    {
        protected override void AddEventExtension(EpcisEvent evt)
        {
            Extension.AddIfAny(XmlEventFormatter.GenerateCustomFields(evt, FieldType.Extension));
        }
    }
}