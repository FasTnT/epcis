using System.Collections.Generic;

namespace FasTnT.Model.MasterDatas
{
    public class MasterDataField
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Value { get; set; }
        public List<MasterDataField> Children { get; set; } = new List<MasterDataField>();
    }
}
