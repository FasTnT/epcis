using FasTnT.Model.Responses;
using System.Collections.Generic;

namespace FasTnT.Model.MasterDatas
{
    public class EpcisMasterData : IEntity
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }

        public IList<MasterDataAttribute> Attributes { get; set; } = new List<MasterDataAttribute>();
    }
}
