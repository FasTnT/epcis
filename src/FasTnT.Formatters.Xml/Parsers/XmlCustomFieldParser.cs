using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Parsers
{
    internal static class XmlCustomFieldParser
    {
        internal static List<CustomField> ParseCustomFields(XElement element, FieldType fieldType)
        {
            var customFields = new List<CustomField>();
            var elements = element?.Elements().Where(x => x.Name.Namespace != XNamespace.Xmlns && x.Name.Namespace != XNamespace.None && x.Name.Namespace != EpcisNamespaces.SBDH && x.Name.Namespace != null);

            foreach (var field in elements ?? new XElement[0])
            {
                customFields.Add(ParseCustomField(field, fieldType));
            }

            return customFields;
        }

        internal static CustomField ParseCustomField(XElement element, FieldType fieldType)
        {
            var field = new CustomField
            {
                Type = fieldType,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName,
                TextValue = element.HasElements ? null : element.Value,
                NumericValue = element.HasElements ? null : double.TryParse(element.Value, out double doubleValue) ? doubleValue : default(double?),
                DateValue = element.HasElements ? null : DateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?),
            };

            if (element.HasElements)
            {
                foreach (var children in element.Elements())
                {
                    field.Children.Add(ParseCustomField(children, fieldType));
                }
            }

            foreach (var attribute in element.Attributes().Where(a => a.Name.LocalName != "xmlns"))
            {
                ParseAttribute(element, field, attribute);
            }

            return field;
        }

        private static void ParseAttribute(XElement element, CustomField field, XAttribute attribute)
        {
            var attributeField = new CustomField
            {
                Type = FieldType.Attribute,
                Name = attribute.Name.LocalName,
                Namespace = attribute.Name.Namespace.NamespaceName,
                TextValue = attribute.Value,
                NumericValue = element.HasElements ? null : double.TryParse(element.Value, out double doubleVal) ? doubleVal : default(double?),
                DateValue = element.HasElements ? null : DateTime.TryParse(element.Value, out DateTime dateVal) ? dateVal : default(DateTime?),
            };

            field.Children.Add(attributeField);
        }
    }
}
