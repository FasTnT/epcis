using FasTnT.Model.MasterDatas;

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
                MasterdataId = MasterdataId,
                MasterdataType = MasterdataType,
                ParentId = ParentId,
                Name = Name,
                Namespace = Namespace,
                Value = Value
            };
        }

        public static MasterDataFieldDto Create(MasterDataField field, int id, int? parentId)
        {
            return new MasterDataFieldDto
            {
                Id = id,
                InternalParentId = parentId,
                MasterdataId = field.MasterdataId,
                MasterdataType = field.MasterdataType,
                ParentId = field.ParentId,
                Name = field.Name,
                Namespace = field.Namespace,
                Value = field.Value
            };
        }
    }
}
