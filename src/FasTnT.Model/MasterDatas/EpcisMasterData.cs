using FasTnT.Model.Responses;
using System.Collections.Generic;

namespace FasTnT.Model.MasterDatas
{
    public class EpcisMasterData : IEntity
    {
        public string Type { get; set; }
        public string Id { get; set; }

        public List<MasterDataAttribute> Attributes { get; set; } = new List<MasterDataAttribute>();
        public List<EpcisMasterDataHierarchy> Children { get; set; } = new List<EpcisMasterDataHierarchy>();
    }
}
