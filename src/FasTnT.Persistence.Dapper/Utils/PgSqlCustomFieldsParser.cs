using FasTnT.Model;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Persistence.Dapper.Utils
{
    internal static class PgSqlCustomFieldsParser
    {
        internal static void ParseFields(IList<CustomField> customFields, int eventId, List<CustomFieldEntity> mappedList, int? parentId = null)
        {
            if (customFields == null || !customFields.Any()) return;

            foreach (var field in customFields)
            {
                var entity = field.Map<CustomField, CustomFieldEntity>(f => { f.EventId = eventId; f.Id = mappedList.Count; f.ParentId = parentId; });
                mappedList.Add(entity);

                ParseFields(field.Children, eventId, mappedList, entity.Id);
            }
        }
    }
}
