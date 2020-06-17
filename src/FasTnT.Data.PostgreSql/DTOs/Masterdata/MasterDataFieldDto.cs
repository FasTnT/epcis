using FasTnT.Model.MasterDatas;
using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class MasterDataFieldDto
    {
        public int Id { get; set; }
        public int? InternalParentId { get; set; }
        public string MasterdataId { get; set; }
        public string MasterdataType { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Value { get; set; }

        public MasterDataField ToMasterDataField()
        {
            return new MasterDataField
            {
                Name = Name,
                Namespace = Namespace,
                Value = Value
            };
        }

        public static MasterDataFieldDto Create(MasterDataField field, MasterDataAttributeDto attribute, int id, int? parentId)
        {
            return new MasterDataFieldDto
            {
                Id = id,
                InternalParentId = parentId,
                MasterdataId = attribute.ParentId,
                MasterdataType = attribute.ParentType,
                ParentId = attribute.Id,
                Name = field.Name,
                Namespace = field.Namespace,
                Value = field.Value
            };
        }
    }
}
