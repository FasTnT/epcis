using FasTnT.Model.Responses;
using System;
using System.Collections.Generic;

namespace FasTnT.Model.MasterDatas
{
    public class EpcisMasterData : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ParentId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public IList<MasterDataAttribute> Attributes { get; set; } = new List<MasterDataAttribute>();
    }
}
