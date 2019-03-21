using FasTnT.Model.MasterDatas;

namespace FasTnT.Persistence.Dapper
{
    public class MasterDataFieldEntity : MasterDataField
    {
        public int Id { get; set; }
        public int? InternalParentId { get; set; }
    }
}
