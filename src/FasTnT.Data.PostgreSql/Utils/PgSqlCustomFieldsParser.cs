using FasTnT.Model;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Data.PostgreSql.Utils
{
    internal static class PgSqlCustomFieldsParser
    {
        internal static void ParseFields(IList<CustomField> customFields, int eventId, List<CustomField> mappedList, int? parentId = null)
        {
            if (customFields == null || !customFields.Any()) return;

            foreach (var field in customFields)
            {
                field.EventId = eventId;
                field.Id = mappedList.Count;
                field.ParentId = parentId;

                mappedList.Add(field);

                ParseFields(field.Children, eventId, mappedList, field.Id);
            }
        }
    }
}
