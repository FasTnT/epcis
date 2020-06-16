using FasTnT.Data.PostgreSql.DTOs;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;

namespace FasTnT.Data.PostgreSql.DapperConfiguration
{
    public static class MasterdataFieldExtensions
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
    }
}
