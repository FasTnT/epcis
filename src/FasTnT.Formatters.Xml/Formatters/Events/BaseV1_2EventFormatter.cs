using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public abstract class BaseV1_2EventFormatter : BaseV1EventFormatter
    {
        protected override void AddEventExtension(EpcisEvent evt)
        {
            Extension.AddIfAny(XmlEventFormatter.GenerateCustomFields(evt, FieldType.EventExtension));
        }
    }
}