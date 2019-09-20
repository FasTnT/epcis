using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Model;
using FasTnT.Model.Events;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Utils;

namespace FasTnT.Formatters.Xml
{
    internal static class XmlHeaderParser
    {
        internal static StandardBusinessHeader Parse(XElement element)
        {
            if (element == null) return null;
            var documentIdentification = element.Element(XName.Get("DocumentIdentification", EpcisNamespaces.SBDH));

            return new StandardBusinessHeader
            {
                CreationDateTime = DateTime.Parse(documentIdentification.Element(XName.Get("CreationDateAndTime", EpcisNamespaces.SBDH))?.Value),
                Standard = documentIdentification.Element(XName.Get("Standard", EpcisNamespaces.SBDH))?.Value,
                Type = documentIdentification.Element(XName.Get("Type", EpcisNamespaces.SBDH))?.Value,
                TypeVersion = documentIdentification.Element(XName.Get("TypeVersion", EpcisNamespaces.SBDH))?.Value,
                InstanceIdentifier = documentIdentification.Element(XName.Get("InstanceIdentifier", EpcisNamespaces.SBDH))?.Value,
                ContactInformations = ParseContacts(element.Elements(XName.Get("Sender", EpcisNamespaces.SBDH))).Union(ParseContacts(element.Elements(XName.Get("Receiver", EpcisNamespaces.SBDH)))).ToList(),
            };
        }

        internal static List<CustomField> ParseCustomFields(XElement element)
        {
            var customFields = new List<CustomField>();
            var elements = element?.Elements().Where(x => x.Name.Namespace != XNamespace.Xmlns && x.Name.Namespace != XNamespace.None && x.Name.Namespace != EpcisNamespaces.SBDH && x.Name.Namespace != null);

            foreach (var field in elements ?? new XElement[0])
            {
                customFields.Add(ParseCustomField(field));
            }

            return customFields;
        }

        private static CustomField ParseCustomField(XElement element)
        {
            var field = new CustomField
            {
                Type = FieldType.HeaderExtension,
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
                    var childrenField = ParseCustomField(children);
                    field.Children.Add(childrenField);
                }
            }

            foreach (var attribute in element.Attributes().Where(a => a.Name.LocalName != "xmlns"))
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

            return field;
        }

        private static IEnumerable<ContactInformation> ParseContacts(IEnumerable<XElement> elements)
        {
            return elements.Select(e => new ContactInformation
            {
                Type = Enumeration.GetByDisplayName<ContactInformationType>(e.Name.LocalName),
                Contact = e.Element(XName.Get("Contact", EpcisNamespaces.SBDH))?.Value,
                ContactTypeIdentifier = e.Element(XName.Get("ContactTypeIdentifier", EpcisNamespaces.SBDH))?.Value,
                EmailAddress = e.Element(XName.Get("EmailAddress", EpcisNamespaces.SBDH))?.Value,
                TelephoneNumber = e.Element(XName.Get("TelephoneNumber", EpcisNamespaces.SBDH))?.Value,
                FaxNumber = e.Element(XName.Get("FaxNumber", EpcisNamespaces.SBDH))?.Value,
                Identifier = e.Element(XName.Get("Identifier", EpcisNamespaces.SBDH))?.Value,
            });
        }
    }
}
