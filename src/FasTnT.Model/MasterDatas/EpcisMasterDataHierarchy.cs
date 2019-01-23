using FasTnT.Model.Responses;

namespace FasTnT.Model.MasterDatas
{
    public class EpcisMasterDataHierarchy : IEntity
    {
        public string Type { get; set; }
        public string ParentId { get; set; }
        public string ChildrenId { get; set; }
    }
}
