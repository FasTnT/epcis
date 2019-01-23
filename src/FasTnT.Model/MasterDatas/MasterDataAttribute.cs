﻿using System.Collections.Generic;

namespace FasTnT.Model.MasterDatas
{
    public class MasterDataAttribute
    {
        public string ParentId { get; set; }
        public string ParentType { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
        public List<MasterDataField> Fields { get; set; } = new List<MasterDataField>();
    }
}
