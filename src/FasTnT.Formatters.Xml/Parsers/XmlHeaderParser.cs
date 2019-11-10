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
