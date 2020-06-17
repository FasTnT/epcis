using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class MasterDataHierarchyDto
    {
        public string Type { get; set; }
        public string ParentId { get; set; }
        public string ChildrenId { get; set; }

        internal static MasterDataHierarchyDto Create(string children, string id, string type)
        {
            return new MasterDataHierarchyDto
            {
                Type = type,
                ParentId = id,
                ChildrenId = children
            };
        }

        internal string ToHierarchy()
        {
            return ChildrenId;
        }
    }
}
