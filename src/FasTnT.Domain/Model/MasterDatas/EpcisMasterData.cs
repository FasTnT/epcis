using System.Collections.Generic;

namespace FasTnT.Model.MasterDatas
{
    public class EpcisMasterData
    {
        public string Type { get; set; }
        public string Id { get; set; }

        public List<MasterDataAttribute> Attributes { get; set; } = new List<MasterDataAttribute>();
        public List<string> Children { get; set; } = new List<string>();
    }
}
