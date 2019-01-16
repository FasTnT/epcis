using System;

namespace FasTnT.Model.MasterDatas
{
    public class MasterDataAttribute
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MasterDataId { get; set; }
        public Guid? ParentId { get; set; }
        public string NameSpace { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
