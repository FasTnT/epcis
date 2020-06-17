using FasTnT.Data.PostgreSql.DTOs;
using FasTnT.Model.Events;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;

namespace FasTnT.Data.PostgreSql.DapperConfiguration
{
    public static class DtoExtensions
    {
        public static IEnumerable<MasterDataFieldDto> ToFlattenedDtos(this IEnumerable<MasterDataField> fields, MasterDataAttributeDto attribute)
        {
            var list = new List<MasterDataFieldDto>();

            foreach (var field in fields)
            {
                list.AddRange(CreateFieldDto(field, (short)list.Count, attribute, null));
            }

            return list;
        }

        private static IEnumerable<MasterDataFieldDto> CreateFieldDto(MasterDataField field, short fieldId, MasterDataAttributeDto attribute, short? parentId)
        {
            var list = new List<MasterDataFieldDto>
            {
                MasterDataFieldDto.Create(field, attribute, fieldId, parentId)
            };

            foreach (var children in field.Children)
            {
                list.AddRange(CreateFieldDto(children, (short)(fieldId + list.Count), attribute, fieldId));
            }

            return list;
        }

        public static IEnumerable<CustomFieldDto> ToFlattenedDtos(this IEnumerable<CustomField> customFields, short eventId, int requestId)
        {
            var list = new List<CustomFieldDto>();

            foreach (var field in customFields)
            {
                list.AddRange(CreateFieldDto(field, (short)list.Count, eventId, requestId, null));
            }

            return list;
        }

        private static IEnumerable<CustomFieldDto> CreateFieldDto(CustomField field, short fieldId, short eventId, int requestId, short? parentId)
        {
            var list = new List<CustomFieldDto>
            {
                CustomFieldDto.Create(field, fieldId, eventId, requestId, parentId)
            };

            foreach (var children in field.Children)
            {
                list.AddRange(CreateFieldDto(children, (short)(fieldId + list.Count), eventId, requestId, fieldId));
            }

            return list;
        }
    }
}
