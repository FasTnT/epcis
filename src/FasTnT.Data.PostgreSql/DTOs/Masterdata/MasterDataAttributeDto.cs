using FasTnT.Model.MasterDatas;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class MasterDataAttributeDto
    {
        public string ParentId { get; set; }
        public string ParentType { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }

        internal static MasterDataAttributeDto Create(MasterDataAttribute masterdata, string id, string type)
        {
            return new MasterDataAttributeDto
            {
                ParentId = id,
                ParentType = type,
                Id = masterdata.Id,
                Value = masterdata.Value
            };
        }
    }
}
